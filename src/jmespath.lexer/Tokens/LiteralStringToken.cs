namespace jmespath.lexer.Tokens;
internal class LiteralStringToken : Token
{
    private string yytext;

    public LiteralStringToken(string yytext)
        : base(TokenType.T_LSTRING, yytext)
    {
        this.yytext = yytext;
    }
}