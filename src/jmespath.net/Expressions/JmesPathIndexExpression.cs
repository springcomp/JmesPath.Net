using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathIndexExpression : JmesPathCompoundExpression
    {
        public override string ExpressionType
            => "index-expression";

        public JmesPathIndexExpression(JmesPathExpression expression, JmesPathExpression specifier)
            : base(expression, specifier)
        {
            System.Diagnostics.Debug.Assert(
                specifier is JmesPathIndex ||
                specifier is JmesPathFilterProjection ||
                specifier is JmesPathFlattenProjection ||
                specifier is JmesPathListWildcardProjection ||
                specifier is JmesPathSliceProjection ||
                false
                );
        }
    }
}