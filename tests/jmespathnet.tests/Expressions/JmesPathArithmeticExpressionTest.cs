using DevLab.JmesPath;
using DevLab.JmesPath.Expressions;
using Xunit;

namespace jmespath.net.tests.Expressions
{
    public class JmesPathArithmeticExpressionTest : JmesPathExpressionsTestBase
    {
        [Theory]
        [InlineData("foo + bar", "{\"foo\": 40, \"bar\": 2}", "42.0")]
        public void JmesPathArithmeticExpression_Transform(string expression, string document, string expected)
            => Assert(new JmesPath().Parse(expression), document, expected);
    }
}