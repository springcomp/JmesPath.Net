using System.Collections;

using DevLab.JmesPath;
using jmespath.lexer;

using PrefixParselet = System.Func<jmespath.lexer.Token, Gratt.Parser<DevLab.JmesPath.IJmesPathGenerator2, jmespath.lexer.TokenType, jmespath.lexer.Token, int, bool>, bool>;
using InfixParselet = System.Func<jmespath.lexer.Token, bool, Gratt.Parser<DevLab.JmesPath.IJmesPathGenerator2, jmespath.lexer.TokenType, jmespath.lexer.Token, int, bool>, bool>;

public static partial class JMESPath
{
    public static bool Parse(string expression, IJmesPathGenerator2 generator)
    {
        var tokens = Tokenizer
            .Tokenize(expression)
#if DEBUG
            .ToArray()
#endif
            ;

        var succeeded = Gratt.Parser.Parse(
            generator,
            0,
            TokenType.EOF,
            Error.Syntax,
            (type, token, _) => Spec.Instance.Prefix(type, token.Location),
            (type, token, _) => Spec.Instance.Infix(type),
            tokens
            );

        if (succeeded)
            generator.OnExpression();

        return succeeded;
    }

    static class Precedence
    {
        public const int T_PIPE = 1;
        public const int T_OR = 2;
        public const int T_AND = 3;

        public const int T_EQ = 5;
        public const int T_GE = 5;
        public const int T_GT = 5;
        public const int T_LE = 5;
        public const int T_LT = 5;
        public const int T_NE = 5;
    
        // everything above stops a projection

        public const int T_NOT = 45;
    }

    static class Parselets
    {
        public static readonly PrefixParselet Error =
            (token, _) => throw JMESPath.Error.Syntax(token);

        // prefix parselets

        public static readonly PrefixParselet Current =
            (_, parser) => { parser.State.OnCurrentNode(); return true; };

        public static readonly PrefixParselet Identifier =
            (token, parser) => { parser.State.OnIdentifier((string)token.Value); return true; };

        public static readonly PrefixParselet Literal =
            (token, parser) => { parser.State.OnLiteralString((string)token.Value); return true; };

        public static readonly PrefixParselet RawString =
            (token, parser) => { parser.State.OnRawString((string)token.Value); return true; };

        public static readonly PrefixParselet IdentifierOrFunctionCall =
            (token, parser) =>
            {
                var identifier = (string)token.Value;

                // attempt to parse function call

                var (tokenType, nextToken) = parser.Peek();
                if (tokenType == TokenType.T_LPAREN)
                    return OnFunctionCall(parser, identifier);

                parser.State.OnIdentifier(identifier);
                return true;
            };

        public static readonly PrefixParselet HashWildcard =
            (token, parser) => { parser.State.OnHashWildcardProjection(); return true; };

        public static readonly PrefixParselet ExpressionType =
            (token, parser) =>
            {
                if (!parser.State.InFunctionArg)
                    throw JMESPath.Error.Syntax(token);

                var succeeded = parser.Parse(0); // TODO: error
                parser.State.OnExpressionType();
                return succeeded;
            };

        public static readonly PrefixParselet NotExpression =
            (_, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_NOT); // TODO: error
                parser.State.OnNotExpression();
                return succeeded;
            };

        public static readonly PrefixParselet ParenExpression =
            (_, parser) =>
            {
                var succeeded = parser.Parse(0); // TODO error
                parser.Read(TokenType.T_RPAREN, parser.Missing(TokenType.T_RPAREN));
                return succeeded;
            };

        public static readonly PrefixParselet MultiSelectHash =
            (_, parser) => OnMultiSelectHash(parser);

        // infix / postfix parselets

        public static InfixParselet PipeExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_PIPE); // TODO error
                parser.State.OnPipeExpression();
                return succeeded;
            };

        public static InfixParselet OrExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_OR); // TODO error
                parser.State.OnOrExpression();
                return succeeded;
            };

        public static InfixParselet AndExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_AND); // TODO error
                parser.State.OnAndExpression();
                return succeeded;
            };

        public static InfixParselet ComparatorEqualExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_EQ); // TODO error
                parser.State.OnComparisonEqual();
                return succeeded;
            };
        public static InfixParselet ComparatorGreaterThanOrEqualExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_GE); // TODO error
                parser.State.OnComparisonGreaterOrEqual();
                return succeeded;
            };
        public static InfixParselet ComparatorGreaterThanExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_GT); // TODO error
                parser.State.OnComparisonGreater();
                return succeeded;
            };
        public static InfixParselet ComparatorLesserThanOrEqualExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_LE); // TODO error
                parser.State.OnComparisonLesserOrEqual();
                return succeeded;
            };
        public static InfixParselet ComparatorLesserThanExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_LT); // TODO error
                parser.State.OnComparisonLesser();
                return succeeded;
            };
        public static InfixParselet ComparatorNotEqualExpression =
            (_, _, parser) =>
            {
                var succeeded = parser.Parse(Precedence.T_NE); // TODO error
                parser.State.OnComparisonNotEqual();
                return succeeded;
            };
    }

    sealed class Spec : IEnumerable
    {
        public static readonly Spec Instance = new Spec
        {
            // register all the parselets for the grammar

            { TokenType.EOF, Parselets.Error },
            { TokenType.E_UNRECOGNIZED, Parselets.Error },

            // prefixed parselets

            { TokenType.T_CURRENT, Parselets.Current },

            { TokenType.T_LSTRING, Parselets.Literal },
            { TokenType.T_QSTRING, Parselets.Identifier },
            { TokenType.T_RSTRING, Parselets.RawString },

            { TokenType.T_USTRING, Parselets.IdentifierOrFunctionCall },

            { TokenType.T_STAR, Parselets.HashWildcard },

            { TokenType.T_ETYPE, Parselets.ExpressionType },
            { TokenType.T_NOT, Parselets.NotExpression },

            { TokenType.T_LBRACE, Parselets.MultiSelectHash },
            { TokenType.T_LPAREN, Parselets.ParenExpression },

            // infix / postfix parselets

            { TokenType.T_PIPE, Precedence.T_PIPE, Parselets.PipeExpression },
            { TokenType.T_OR, Precedence.T_OR, Parselets.OrExpression },
            { TokenType.T_AND, Precedence.T_AND, Parselets.AndExpression },

            { TokenType.T_EQ, Precedence.T_EQ, Parselets.ComparatorEqualExpression },
            { TokenType.T_GE, Precedence.T_GE, Parselets.ComparatorGreaterThanOrEqualExpression },
            { TokenType.T_GT, Precedence.T_GT, Parselets.ComparatorGreaterThanExpression },
            { TokenType.T_LE, Precedence.T_LE, Parselets.ComparatorLesserThanOrEqualExpression },
            { TokenType.T_LT, Precedence.T_LT, Parselets.ComparatorLesserThanExpression },
            { TokenType.T_NE, Precedence.T_NE, Parselets.ComparatorNotEqualExpression },
        };

        readonly Dictionary<TokenType, PrefixParselet> _prefixes = new();
        readonly Dictionary<TokenType, (int, InfixParselet)> _infixes = new();

        Spec() { }

        void Add(TokenType tokenType, PrefixParselet prefix)
            => _prefixes.Add(tokenType, prefix);
        void Add(TokenType tokenType, int precedence, InfixParselet infix)
            => _infixes.Add(tokenType, (precedence, infix));

        public PrefixParselet Prefix(TokenType tokenType, LexLocation? location)
        {
            if (!_prefixes.ContainsKey(tokenType))
                throw Error.Syntax(tokenType, location);

            return _prefixes[tokenType];
        }

        public (int, InfixParselet)? Infix(TokenType tokenType)
        {
            if (!_infixes.TryGetValue(tokenType, out var v))
                return default;
            var (precedence, infix) = v;
            return (precedence, infix);
        }

        public IEnumerator GetEnumerator()
            => _prefixes.Cast<object>()
            .Concat(_infixes.Cast<object>())
            .GetEnumerator()
            ;
    }

    #region Implementation

    private static bool OnFunctionCall(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser, string functionName)
    {
        parser.Read(); // T_LPAREN

        var succeeded = false;

        parser.State.PushFunction();

        do
        {
            var (tokenType, _) = parser.Peek();
            if (tokenType == TokenType.T_RPAREN)
            {
                succeeded = true;
                break;
            }

            while (true)
            {
                // parse argument

                succeeded = parser.Parse(0); // TODO: error
                parser.State.AddFunctionArg();

                // move to next argument 

                (tokenType, var nextToken) = parser.Peek();
                if (tokenType == TokenType.T_RPAREN)
                    break;

                if (tokenType != TokenType.T_COMMA)
                    throw Error.Syntax(nextToken);

                parser.Read();
            }

        } while (false);

        if (succeeded)
        {
            parser.State.PopFunction(functionName);
            parser.Read(TokenType.T_RPAREN, parser.Missing(TokenType.T_RPAREN));
        }

        return succeeded;
    }

    private static bool OnMultiSelectHash(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser)
    {
        var succeeded = false;

        parser.State.PushMultiSelectHash();

        while (true)
        {
            succeeded = OnKeyValExpression(parser); // TODO error

            parser.State.AddMultiSelectHashExpression();

            // move to next keyval-expression

            var (tokenType, nextToken) = parser.Peek();

            if (tokenType == TokenType.T_RBRACE)
                break;

            if (tokenType != TokenType.T_COMMA)
                throw JMESPath.Error.Syntax(nextToken);

            parser.Read();
        }

        if (succeeded)
        {
            parser.State.PopMultiSelectHash();
            parser.Read(TokenType.T_RBRACE, parser.Missing(TokenType.T_RBRACE));
        }

        return succeeded;
    }

    private static bool OnKeyValExpression(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser)
    {
        var (tokenType, nextToken) = parser.Peek();
        if (!new[] { TokenType.T_USTRING, TokenType.T_QSTRING, }.Contains(tokenType))
            throw JMESPath.Error.Syntax(nextToken);

        // parse identifier

        var succeeded = parser.Parse(0); // TODO error

        parser.Read(TokenType.T_COLON, delegate { throw JMESPath.Error.Missing(TokenType.T_COLON, parser.GetLocation()); });

        // parse expression

        succeeded = parser.Parse(0); //TODO error

        return succeeded;
    }

    #endregion
}

