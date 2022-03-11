using jmespath.lexer.Utils;
using JsonCheckerTool;

namespace jmespath.lexer.Tokens;
internal class LiteralStringToken : Token
{
    private readonly string value_;

    public LiteralStringToken(string rawText)
        : base(TokenType.T_LSTRING, rawText)
    {
        System.Diagnostics.Debug.Assert(rawText.Length >= 2);
        System.Diagnostics.Debug.Assert(rawText.StartsWith("`"));
        System.Diagnostics.Debug.Assert(rawText.EndsWith("`"));

        var literal = StringUtil.UnescapeLiteral(rawText);
        CheckValidJson(literal);
        value_ = literal;
    }

    private static void CheckValidJson(string literal)
    {
        var checker = new JsonChecker();
        var lws = true;
        var scalar = false;
        foreach (var ch in literal)
        {
            if (lws) // leading white space?
            {
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break; // ignore leading white space
                    default:   // first non-white-space
                        lws = false;
                        // if it's a scalar then embed in an array
                        if (scalar = ch != '[' && ch != '{')
                            checker.Check('[');
                        break;
                }
            }
            checker.Check(ch);
        }
        if (scalar)
            checker.Check(']');
        checker.FinalCheck();
    }

    public override object Value => value_;
}