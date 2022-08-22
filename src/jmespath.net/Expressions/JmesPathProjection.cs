using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public abstract class JmesPathProjection : JmesPathSimpleExpression
    {
        public JmesPathProjection(JmesPathExpression child)
            :base(child)
        {
        }

        public abstract JmesPathArgument Project(JmesPathArgument argument);

        protected override JmesPathArgument Transform(JToken json)
            => Project(json);
    }
}