namespace jmespath.lexer.Tokens;
internal class QuotedStringToken : Token
{
    private string yytext;

    public QuotedStringToken(string yytext)
        : base(TokenType.T_QSTRING, yytext)
    {
        this.yytext = yytext;
    }
}