using jmespath.net.tests.Parser;

namespace jmespath.net.tests.Functions
{
    using FactAttribute = Xunit.FactAttribute;

    public class MaxByfunctionTest : ParserTestBase
    {
        [Fact]
        public void MaxBy()
        {
            const string expected = @"{""match_number"":1,""alliances"":{""red"":{""score"":120},""blue"":{""score"":60}}}";

            string json = $"{{\"matches\": [{expected}]}}";

            const string expression = "max_by(matches, &alliances.red.score)";

            Assert(expression, json, expected);
        }
    }
}
