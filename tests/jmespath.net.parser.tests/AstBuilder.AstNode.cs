namespace jmespath.net.parser.tests.Blocks
{
    public partial class AstBuilder
    {
        public sealed class AstBlock
        { 
            public AstClosure Closure { get; set; }
        }

        public sealed class AstClosure
        { 
            public string Identifier { get; set; }
            public AstNode Expression { get; set; }
        }

        public sealed class AstNode
        {
            private readonly string type_;
            private readonly string expression_;
            private readonly AstNode left_;
            private readonly AstNode right_;

            public AstNode(string type, string expression)
            {
                type_ = type;
                expression_ = expression;
            }
            public AstNode(AstNode left, AstNode right)
            {
                left_ = left;
                right_ = right;
            }
            public string Type => type_;
            public string Expression => expression_;

            public AstNode Left => left_;
            public AstNode Right => right_;

            public AstBlock Block { get; set; }

            public override string ToString()
                => $"{Type}: {Expression}";
        }
    }
}