namespace jmespath.lexer.Tokens;
internal class RawStringToken : Token
{
    private string yytext;

    public RawStringToken(string yytext)
        : base(TokenType.T_RSTRING, yytext)
    {
        this.yytext = yytext;
    }
}