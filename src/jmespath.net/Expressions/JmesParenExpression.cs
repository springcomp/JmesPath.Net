using DevLab.JmesPath.Interop;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesParenExpression : JmesPathExpression
    {
        private readonly JmesPathExpression expression_;

        public JmesParenExpression(JmesPathExpression expression)
        {
            expression_ = expression;
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            return expression_.Transform(json);
        }

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);
            expression_.Accept(visitor);
        }
    }
}