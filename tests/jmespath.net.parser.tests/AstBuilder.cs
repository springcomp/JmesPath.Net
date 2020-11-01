namespace jmespath.net.parser.tests.Blocks
{
    using DevLab.JmesPath;
    using System.Collections.Generic;

    public class AstBuilder : IJmesPathGenerator
    {
        readonly Stack<AstNode> expressions_
            = new Stack<AstNode>()
            ;

        AstNode expression_;

        public AstNode Root => expression_;

        public virtual void AddFunctionArg()
        {
        }

        public virtual void AddMultiSelectHashExpression()
        {
        }

        public virtual void AddMultiSelectListExpression()
        {
        }

        public bool IsProjection()
        {
            return false;
        }

        public virtual void OnAndExpression()
        {
        }

        public virtual void OnComparisonEqual()
        {
        }

        public virtual void OnComparisonGreater()
        {
        }

        public virtual void OnComparisonGreaterOrEqual()
        {
        }

        public virtual void OnComparisonLesser()
        {
        }

        public virtual void OnComparisonLesserOrEqual()
        {
        }

        public virtual void OnComparisonNotEqual()
        {
        }

        public virtual void OnCurrentNode()
        {
        }

        public void OnExpression()
        {
            if (expression_ == null)
                expression_ = expressions_.Pop();
        }

        public virtual void OnExpressionType()
        {
        }

        public virtual void OnFilterProjection()
        {
        }

        public virtual void OnFlattenProjection()
        {
        }

        public virtual void OnHashWildcardProjection()
        {
        }

        public virtual void OnIdentifier(string name)
        {
            Prolog();

            var expression = new AstNode("identifier", name);
            expressions_.Push(expression);
        }

        public virtual void OnIndex(int index)
        {
        }

        public virtual void OnIndexExpression()
        {
        }

        public virtual void OnListWildcardProjection()
        {
        }

        public virtual void OnLiteralString(string literal)
        {
        }

        public virtual void OnNotExpression()
        {
        }

        public virtual void OnOrExpression()
        {
        }

        public virtual void OnPipeExpression()
        {
        }

        public virtual void OnRawString(string value)
        {
        }

        public virtual void OnSliceExpression(int? start, int? stop, int? step)
        {
        }

        public virtual void OnSubExpression()
        {
            Prolog();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            var expression = new AstNode(left, right);

            expressions_.Push(expression);
        }

        public virtual void PopFunction(string name)
        {
        }

        public virtual void PopMultiSelectHash()
        {
        }

        public virtual void PopMultiSelectList()
        {
        }

        public virtual void PushFunction()
        {
        }

        public virtual void PushMultiSelectHash()
        {
        }

        public virtual void PushMultiSelectList()
        {
        }
        private bool Prolog()
        {
            if (expression_ != null)
            {
                expressions_.Push(expression_);
                expression_ = null;
                return true;
            }

            return false;
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

            public override string ToString()
                => $"{Type}: {Expression}";
        }
    }
}