using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathReduceProjection : JmesPathProjection
    {
        private readonly JmesPathExpression seed_;

        public JmesPathReduceProjection(JmesPathExpression seed)
        {
            seed_ = seed;
        }

        protected override JmesPathArgument Project(JmesPathArgument argument)
        {
            if (argument.IsProjection)
                return argument;

            if (!(argument.Token is JArray array))
                return null;

            var seed = seed_.Transform(argument).AsJToken();
            scopes_.PushScope(new JObject { { "$", seed } });

            var arguments = array.Select(e => new JmesPathArgument(e));
            var projection = new JmesPathArgument(arguments);
            projection.MakeReduction();

            return projection;
        }

        protected override string Format()
            => $"[%{seed_}]";
    }
}