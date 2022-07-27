using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathRootNodeExpression : JmesPathExpression
    {
        protected override JmesPathArgument Transform(JToken json)
            => accumulator_.Accumulator;
    }
}