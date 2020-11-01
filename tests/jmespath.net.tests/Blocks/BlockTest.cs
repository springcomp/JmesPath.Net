namespace jmespath.net.tests.Parser
{
    using FactAttribute = Xunit.FactAttribute;

    public class BlockTest
    {
        [Fact]
        public void ParseEmptyBlock()
        {
            const string expression = "{% %} foo";
        }
    }
}