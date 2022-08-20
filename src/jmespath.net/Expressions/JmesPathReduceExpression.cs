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
            accumulator_.PushSeed(seed);

            var arguments = array.Select(e => new JmesPathArgument(e));
            var projection = new JmesPathArgument(arguments);
            projection.MakeReduction();

            return projection;
        }

        protected override string Format()
            => $"[%{seed_}]";
    }

    public sealed class JmesPathReduceAccumulator : JmesPathExpression
    {
        protected override JmesPathArgument Transform(JToken json)
            => accumulator_.Accumulator;

        protected override string Format()
            => "$";
    }

    public interface IReduceAccumulator
    {

        void PushSeed(JToken seed);
        void PopSeed();

        JToken Accumulator { get; set; }
    }
    public sealed class ReduceAccumulator : IReduceAccumulator
    {
        private readonly Stack<JToken> scopes_
            = new Stack<JToken>()
            ;

        public void PopSeed()
            => scopes_.Pop();

        public void PushSeed(JToken seed)
            => scopes_.Push(seed);

        public JToken Accumulator
        {
            get => scopes_.Count > 0 ? scopes_.Peek() ?? JTokens.Null : JTokens.Null;
            set
            {
                if (scopes_.Count != 0)
                {
                    scopes_.Pop();
                    scopes_.Push(value);
                }
            }
        }
    }
}