using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

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
            if (argument.IsProjection)
                argument = argument.AsJToken();

            var array = argument.Token as JArray;
            if (array == null)
                return JTokens.Null;

            var acc = acc_?.Transform(argument).AsJToken() ?? JTokens.Null;

            accumulator_.PushSeed(acc);

            var elements = array
                .Select(e => new JmesPathArgument(e))
                .ToList()
                ;

            var projection = new JmesPathArgument(elements);
            projection.Accumulator = acc;

            return projection;
        }
    }
}