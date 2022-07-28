using jmespath.net.tests.Parser;
using Xunit;

namespace jmespath.net.tests.Expressions
{
    public class JmesPathReduceExpressionTest : ParserTestBase
    {
        [Theory]
        [InlineData("foo[%] -> $||@", "{ \"foo\": [true, false, false] }", "true")]
        [InlineData("[%] -> `true`", "[true, false, false]", "true")]
        [InlineData("[% `false`] -> @ || @", "[true, false, false]", "false")]
        [InlineData("[% `false`] -> $ || @", "[true, false, false]", "true")] // any
        [InlineData("[% `false`] -> $ || @", "[false, false, false]", "false")] // any
        [InlineData("[% `true` ] -> $ && @", "[true, false, false]", "false")] // all
        [InlineData("[% `true` ] -> $ && @", "[true, true, true]", "true")] // all
        [InlineData("[?contains(@, `2`)]|[% `0` ] -> max([$, length(@)])", "[[1, 2, 3], [2, 3], [1, 3]]", "3")] 
        public void JmesPathReduceExpression(string expression, string json, string expected)
            => Assert(expression, json, expected);
    }
}