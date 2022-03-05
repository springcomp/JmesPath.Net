namespace jmespath.lexer.Tokens;
internal class NumberToken : Token
{
    private string yytext;

    public NumberToken(string yytext)
        : base(TokenType.T_NUMBER, yytext)
    {
        this.yytext = yytext;
    }
}