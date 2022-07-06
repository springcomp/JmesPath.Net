using jmespath.lexer.Utils;

namespace jmespath.lexer.Tokens;
internal class QuotedStringToken : Token
{
    private readonly string value_;

    public QuotedStringToken(string rawText)
        : base(TokenType.T_QSTRING, rawText)
    {
        System.Diagnostics.Debug.Assert(rawText.Length >= 2);
        System.Diagnostics.Debug.Assert(rawText.StartsWith("\""));
        System.Diagnostics.Debug.Assert(rawText.EndsWith("\""));

        value_ = StringUtil.Unwrap(rawText);
    }

    public override object Value => value_;
}