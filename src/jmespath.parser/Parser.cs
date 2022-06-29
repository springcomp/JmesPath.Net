using System.Collections;

using DevLab.JmesPath;
using jmespath.lexer;

using PrefixParselet = System.Func<jmespath.lexer.Token, Gratt.Parser<DevLab.JmesPath.IJmesPathGenerator, jmespath.lexer.TokenType, jmespath.lexer.Token, int, bool>, bool>;
using InfixParselet = System.Func<jmespath.lexer.Token, bool, Gratt.Parser<DevLab.JmesPath.IJmesPathGenerator, jmespath.lexer.TokenType, jmespath.lexer.Token, int, bool>, bool>;

public static partial class JMESPath
{
    public static bool Parse(string expression, IJmesPathGenerator generator)
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
    }

    static class Parselets
    {
        public static readonly PrefixParselet Error =
            (token, _) => throw JMESPath.Error.Syntax(token);

        public static readonly PrefixParselet Identifier =
            (token, parser) => { parser.State.OnIdentifier((string)token.Value); return true; };

        public static readonly PrefixParselet Literal =
            (token, parser) => { parser.State.OnLiteralString((string)token.Value); return true; };

        public static readonly PrefixParselet RawString =
            (token, parser) => { parser.State.OnRawString((string)token.Value); return true; };
    }

    sealed class Spec : IEnumerable
    {
        public static readonly Spec Instance = new Spec
        {
            // register all the parselets for the grammar

            { TokenType.EOF, Parselets.Error },
            { TokenType.E_UNRECOGNIZED, Parselets.Error },

            // prefixed parselets

            { TokenType.T_LSTRING, Parselets.Literal },
            { TokenType.T_QSTRING, Parselets.Identifier },
            { TokenType.T_USTRING, Parselets.Identifier },
            { TokenType.T_RSTRING, Parselets.RawString },

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

