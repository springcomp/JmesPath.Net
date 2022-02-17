using jmespath.net.tests.Parser;
using Xunit;

namespace jmespath.net.tests.Functions
{
    public class LetFunctionTest : ParserTestBase
    {
        [Fact]
        public void JmesPathLetCurrentScope()
        {
            // JEP-11 https://github.com/jmespath/jmespath.site/pull/6/commits/ae5ad3f590ae92c9789f3e5cacd99726ea028b74#diff-8f58dd34123874837acec4100110fb28bb1c8df3c4a9e81408e9c4a4775e86aeR128
            // search(let({a: `x`}, &b), {"b": "y"}) -> "y"

            const string json = "{\"b\": \"y\"}";
            const string expression = "let({a: `x`}, &b)";
            const string expected = "\"y\"";

            Assert(expression, json, expected);
        }
    }
}