using DevLab.JmesPath;
using DevLab.JmesPath.Functions;
using DevLab.JmesPath.Utils;
using jmespath.net.tests.Parser;
using Xunit;

namespace jmespathnet.tests.Functions
{
    using FactAttribute = Xunit.FactAttribute;

    public class ReduceFunctionTest : ParserTestBase
    {
        protected override void RegisterFunction(JmesPath parser)
        {
            parser
                .FunctionRepository
                .Register<ReduceFunction>();

            base.RegisterFunction(parser);
        }

        [Theory]
        [InlineData("reduce(@, `false`, (acc, cur) => acc || cur)", "[true, false, false]", "true")] // any
        [InlineData("reduce(@, `false`, (acc, cur) => acc || cur)", "[false, false, false]", "false")] // any
        [InlineData("reduce(@, `true`, (acc, cur) => acc && cur)", "[true, false, false]", "false")] // all
        [InlineData("reduce(@, `true`, (acc, cur) => acc && cur)", "[true, true, true]", "true")] // all
        [InlineData("reduce([?contains(@, `2`)], `0`, (acc, cur) => max([acc, length(cur)]))", "[[1, 2, 3], [2, 3], [1, 4]]", "3")]
        public void JmesPathReduceFunctionTests(string expression, string json, string expected)
            => Assert(expression, json, expected);
    }
}