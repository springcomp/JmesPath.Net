using DevLab.JmesPath;
using System.IO;
using System.Text;
using static jmespath.net.parser.tests.Blocks.AstBuilder;

namespace jmespath.net.parser.tests.Blocks
{
    public class ParserTestBase
    {
        public static AstNode Parse(string expression)
            => Parse(expression, new AstBuilder());

        public static AstNode Parse(string expression, AstBuilder generator)
        {
            var encoding = Encoding.UTF8;
            using (var stream = new MemoryStream(encoding.GetBytes(expression)))
                Parser.Parse(stream, encoding, generator);
            return generator.Root;
        }
    }
}