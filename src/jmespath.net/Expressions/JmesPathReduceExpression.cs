using DevLab.JmesPath.Utils;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathReduceExpression : JmesPathProjection
    {
        private readonly JmesPathExpression acc_;

        public JmesPathReduceExpression(JmesPathExpression seed)
        {
            acc_ = seed;
        }

        public override JmesPathArgument Project(JmesPathArgument argument)
        {
            return JTokens.Null;
        }
    }
}