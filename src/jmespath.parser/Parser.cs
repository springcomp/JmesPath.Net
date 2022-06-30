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
        // everything above stops a projection

        public const int T_NOT = 45;
    }

    static class Parselets
    {
        public static readonly PrefixParselet Error =
            (token, _) => throw JMESPath.Error.Syntax(token);

        public static readonly PrefixParselet Current =
            (_, parser) => { parser.State.OnCurrentNode(); return true; };

        public static readonly PrefixParselet Identifier =
            (token, parser) => { parser.State.OnIdentifier((string)token.Value); return true; };

        public static readonly PrefixParselet Literal =
            (token, parser) => { parser.State.OnLiteralString((string)token.Value); return true; };

        public static readonly PrefixParselet RawString =
            (token, parser) => { parser.State.OnRawString((string)token.Value); return true; };

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
            { TokenType.T_USTRING, Parselets.Identifier },
            { TokenType.T_RSTRING, Parselets.RawString },

            { TokenType.T_STAR, Parselets.HashWildcard },

            { TokenType.T_ETYPE, Parselets.ExpressionType },
            { TokenType.T_NOT, Parselets.NotExpression },

            { TokenType.T_LPAREN, Parselets.ParenExpression },

            // infix / postfix parselets

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
                throw JMESPath.Error.Syntax(tokenType, location);

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

    #endregion
}

