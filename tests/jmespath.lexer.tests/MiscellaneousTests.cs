using Xunit;

namespace jmespath.lexer.tests;

public class MiscellaneousTests
{
    [Fact]
    public void IsWhitespace()
    {
        Assert.True(Scanner.IsWhitespace(' '));
        Assert.True(Scanner.IsWhitespace('\b'));
        Assert.True(Scanner.IsWhitespace('\f'));
        Assert.True(Scanner.IsWhitespace('\n'));
        Assert.True(Scanner.IsWhitespace('\r'));
        Assert.True(Scanner.IsWhitespace('\t'));
    }
}
