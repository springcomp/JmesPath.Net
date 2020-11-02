namespace jmespath.net.parser.tests.Blocks
{
    using System;
    using Xunit;
    using static jmespath.net.parser.tests.Blocks.AstBuilder;
    using FactAttribute = Xunit.FactAttribute;

    public class BlockTest : ParserTestBase
    {
        [Fact]
        public void ParseEmptyBlock()
        {
            const string expression = "items[]. {% expression := id %} children";
            var ast = Parse(expression);

            Expect(ast, type: null, expression: null, "identifier", "identifier");
        }

        private void Expect(AstNode ast, string type, string expression, string leftType, string rightType)
        { 
            if (!String.IsNullOrWhiteSpace(leftType))
                Assert.Equal("identifier", ast.Left.Type);
            if (!String.IsNullOrWhiteSpace(rightType))
                Assert.Equal("identifier", ast.Right.Type);
        }
    }
}