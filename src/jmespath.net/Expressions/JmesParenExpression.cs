using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesParenExpression : JmesPathExpression
    {
        private readonly JmesPathExpression expression_;

        public override string ExpressionType
            => "paren-expression";

        public JmesParenExpression(JmesPathExpression expression)
        {
            expression_ = expression;
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            return expression_.Transform(json);
        }
    }
}