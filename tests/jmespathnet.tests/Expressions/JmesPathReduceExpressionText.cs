using DevLab.JmesPath;
using Xunit;

namespace jmespath.net.tests.Expressions
{
    public class JmesPathReduceExpressionText : JmesPathExpressionsTestBase
    {
        [Theory]
        [InlineData("$", "[1, 2, 3]", "[1,2,3]")]
        [InlineData("[%`0`].($)", "[1, 2, 3]", "0")]
        [InlineData("[%`0`].(@)", "[1, 2, 3]", "3")]
        [InlineData("[%`0`].(@ + $)", "[1, 2, 3]", "6.0")]
        [InlineData("[[`1`, `2`], [`3`, `4`]][]|[%`0`].(@ + $)", "[]", "10.0")]
        [InlineData("[[`1`, `2`], [`3`, `4`]][%`0`].($ + [%`0`].(@ + $))", "[]", "10.0")]
        public void JmesPathReduceExpression_Evaluate(string expression, string json, string expected)
            => Assert(new JmesPath().Parse(expression), json, expected);
    }
}