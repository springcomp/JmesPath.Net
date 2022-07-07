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

        public const int T_FLATTEN = 9;
    
        // everything above stops a projection

        public const int T_FILTER = 21;

        public const int T_LBRACKET = 35;
        public const int T_DOT = 40;
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

        public static readonly PrefixParselet BracketSpecifier =
            (_, parser) => OnBracketSpecifier(parser, postfix: false);
        public static readonly PrefixParselet FlattenProjection =
            (_, parser) => OnFlattenProjection(parser, postfix: false);
        public static readonly PrefixParselet FilterProjection =
            (_, parser) => OnFilterProjection(parser, postfix: false);

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

        public static readonly InfixParselet BracketSpecifierPostfix =
            (_, _, parser) => OnBracketSpecifier(parser, postfix: true);
        public static readonly InfixParselet FlattenProjectionPostfix =
            (_, _, parser) => OnFlattenProjection(parser, postfix: true);
        public static readonly InfixParselet FilterProjectionPostfix =
            (_, _, parser) => OnFilterProjection(parser, postfix: true);

        public static InfixParselet SubExpression =
            (_, _, parser) => OnSubExpression(parser);
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

            { TokenType.T_LBRACKET, Parselets.BracketSpecifier },
            { TokenType.T_FLATTEN, Parselets.FlattenProjection },
            { TokenType.T_FILTER, Parselets.FilterProjection },

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

            { TokenType.T_LBRACKET, Precedence.T_LBRACKET, Parselets.BracketSpecifierPostfix },
            { TokenType.T_FLATTEN, Precedence.T_FLATTEN, Parselets.FlattenProjectionPostfix },
            { TokenType.T_FILTER, Precedence.T_FILTER, Parselets.FilterProjectionPostfix },

            { TokenType.T_DOT, Precedence.T_DOT, Parselets.SubExpression },
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

    private static bool IsTokenInvalid(TokenType kind)
        => kind == TokenType.EOF || kind == TokenType.E_UNRECOGNIZED;

    private static bool OnBracketSpecifier(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser, bool postfix = true)
    {
        var succeeded = false;

        do
        {
            var (k1, t1) = parser.Lookahead();
            var (k2, _) = IsTokenInvalid(k1) ? (k1, t1) : parser.Lookahead(1);

            if (k1 == TokenType.T_COLON || k2 == TokenType.T_COLON)
            {
                var parts = OnSliceExpression(parser, new Token?[3] { null, null, null });
                parser.State.OnSliceExpression((int?)parts[0]?.Value, (int?)parts[1]?.Value, (int?)parts[2]?.Value);
                succeeded = true;
                break;
            }

            else if (k1 == TokenType.T_NUMBER)
            {
                var number = parser.Read(TokenType.T_NUMBER, delegate { throw new Exception(); });
                parser.State.OnIndex((int)number.Value);
                succeeded = true;
                break;
            }

            else if (k1 == TokenType.T_STAR && k2 == TokenType.T_RBRACKET)
            {
                parser.Read();

                parser.State.OnListWildcardProjection();

                succeeded = true;
                break;
            }

            succeeded = OnMultiSelectList(parser);
            System.Diagnostics.Debug.Assert(!postfix);

        } while (false);

        if (succeeded)
        {
            if (postfix)
            {
                var expressionType = parser.State.ExpressionType;
                if (!new[]{
                        "flatten-projection",
                        "index",
                        "list-filter-expression",
                        "list-wildcard-projection",
                        "slice-projection",
                    }.Contains(expressionType))
                {
                    throw Error.Syntax(expressionType, parser.GetLocation());
                }
                parser.State.OnIndexExpression();
            }
            parser.Read(TokenType.T_RBRACKET, parser.Missing(TokenType.T_RBRACKET));
        }

        return succeeded;
    }

    private static bool OnMultiSelectList(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser)
    {
        var succeeded = false;

        parser.State.PushMultiSelectList();

        while (true)
        {
            succeeded = parser.Parse(0); // TODO error
            parser.State.AddMultiSelectListExpression();

            // move to next expression

            var (tokenType, nextToken) = parser.Peek();

            if (tokenType == TokenType.T_RBRACKET)
                break;

            if (tokenType != TokenType.T_COMMA)
                throw Error.Syntax(nextToken);

            parser.Read();
        }

        if (succeeded)
            parser.State.PopMultiSelectList();

        return succeeded;
    }

    private static Token?[] OnSliceExpression(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser, Token?[] tokens)
    {
        System.Diagnostics.Debug.Assert(tokens.Length == 3);

        var index = 0;
        while (index < 3)
        {
            var (kind, token) = parser.Peek();
            if (kind == TokenType.T_RBRACKET)
                break;

            if (kind == TokenType.T_COLON)
            {
                index++;
                if (index == 3)
                    throw Error.Syntax(token);

                parser.Read();
            }

            else if (kind == TokenType.T_NUMBER)
            {
                tokens[index] = token;
                parser.Read();
            }

            else
                throw Error.Syntax(token);
        }

        if (tokens[2] != null && (int)(tokens[2]!.Value) == 0)
            throw Error.Syntax("invalid-value: a slice projection step cannot be 0.", tokens[2]!.Type, tokens[2]!.Location);

        return tokens;
    }

    private static bool OnFlattenProjection(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser, bool postfix = true)
    {
        parser.State.OnFlattenProjection();
        if (postfix)
            parser.State.OnIndexExpression();

        return true;
    }

    private static bool OnFilterProjection(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser, bool postfix = true)
    {
        var succeeded = parser.Parse(0); // TODO: error

        parser.State.OnFilterProjection();
        if (postfix)
            parser.State.OnIndexExpression();

        parser.Read(TokenType.T_RBRACKET, parser.Missing(TokenType.T_RBRACKET));

        return succeeded;
    }

    private static bool OnSubExpression(Gratt.Parser<IJmesPathGenerator2, TokenType, Token, int, bool> parser)
    {
        var succeeded = false;

        // special case: "[*]" on the right-hand-side of a sub-expression is a multi-select-list

        var (k1, t1) = parser.Lookahead(0);
        var (k2, t2) = k1 == TokenType.EOF ? (k1, t1) : parser.Lookahead(1);
        var (k3, _) = k2 == TokenType.EOF ? (k2, t2) : parser.Lookahead(2);

        if (k1 == TokenType.T_LBRACKET && k2 == TokenType.T_STAR && k3 == TokenType.T_RBRACKET)
        {
            parser.Read();
            succeeded = OnMultiSelectList(parser); // TODO error
            parser.Read(TokenType.T_RBRACKET, parser.Missing(TokenType.T_RBRACKET));
        }
        else
        {
            succeeded = parser.Parse(Precedence.T_DOT); // TODO error
        }

        parser.State.OnSubExpression();
        return succeeded;
    }

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

