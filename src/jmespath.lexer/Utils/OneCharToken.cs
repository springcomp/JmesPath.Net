namespace jmespath.lexer.Utils;

using T = TokenType;

internal sealed class OneCharToken
{
    private const T __ = T.EOF;

    private static readonly T[] classes = new T[128] {
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,

                    __,         __,        __,           __,        __,           __,      __, __,
            T.T_LPAREN, T.T_RPAREN,  T.T_STAR,           __, T.T_COMMA,           __, T.T_DOT, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __, T.T_COLON,           __,        __,           __,      __, __,

           T.T_CURRENT,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __, T.T_RBRACKET,      __, __,

                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,           __,        __,           __,      __, __,
                    __,         __,        __,   T.T_LBRACE,        __,   T.T_RBRACE,      __, __,
    };

    /// <summary>
    /// Returns the unambiguous token type
    /// associated with the specified single-character token
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T GetTokenType(char input)
        => classes[32 + (input - ' ')];
}

