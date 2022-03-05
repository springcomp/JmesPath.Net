using jmespath.lexer.StateMachines;

namespace jmespath.lexer;

public sealed class Scanner
{
    private static readonly Token T_EOF = Token.Create(TokenType.EOF, String.Empty);

    private readonly string input_;

    private int position_ = 0;
    private int line_ = 0;
    private int column_ = 0;

    private static readonly StateMachine?[] machines_ = new StateMachine?[(int)TokenType.T_LISTWILDCARD]
    {
        null, null,                 null, null, null, null, null, null,
        null, null,                 null, null, null, null, null, null,
        null, null,                 null, null, null, null, null, null,
        null, null, new UnquotedString(), null, null, null, null, null,
        null,
    };

    public Scanner(string text)
    {
        input_ = text;
    }

    public Token GetNextToken()
    {
        if (position_ >= input_.Length)
            return T_EOF;

        var c = input_[position_];

        if (Char.IsAscii(c) && Char.IsLetter(c))
            return GetNextUnquotedString();

        return T_EOF;
    }

    private Token GetNextUnquotedString()
    {
        System.Diagnostics.Debug.Assert(machines_[(int)TokenType.T_USTRING] != null);
        StateMachine machine = machines_[(int)TokenType.T_USTRING]!;

        var span = input_.AsSpan(position_);
        var match = machine.Match(span);
        if (match.Success)
        {
            var text = match.MatchedText;

            var token = Token.Create(TokenType.T_USTRING, text);
            token.Location = new LexLocation(line_, column_, line_, column_ + text.Length);

            position_ += text.Length;
            column_ += text.Length;

            return token;
        }

        return T_EOF;
    }
}
