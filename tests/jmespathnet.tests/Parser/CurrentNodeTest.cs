
namespace jmespath.net.tests.Parser
{
    using FactAttribute = Xunit.FactAttribute ;

    public class CurrentNodeTest : ParserTestBase
    {
        [Fact]
        public void ParseCurrentNode()
        {
            Assert("[*].{a:@.age}", "[{\"age\": 20,\"other\": \"foo\",\"name\": \"Bob\"},{\"age\": 25,\"other\": \"bar\",\"name\": \"Fred\"},{\"age\": 30,\"other\": \"baz\",\"name\": \"George\"}]", "[{\"a\":20},{\"a\":25},{\"a\":30}]");
        }
    }
}