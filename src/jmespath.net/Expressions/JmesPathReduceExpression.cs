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

        public override JmesPathArgument Project(JmesPathArgument argument)
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

        //protected override JmesPathArgument Transform(JToken json)
        //{
        //    var array = json as JArray;
        //    if (array == null || array.Count == 0)
        //        return JTokens.Null;

        //    var acc = seed_?.Transform(json).AsJToken() ?? JTokens.Null;

        //    try
        //    {
        //        accumulator_.PushSeed(acc);
        //        foreach (var element in array)
        //        {
        //            var result = Right.Transform(element).AsJToken();
        //            accumulator_.Accumulator = result;
        //        }
        //        acc = accumulator_.Accumulator;
        //    }
        //    finally
        //    {
        //        accumulator_.PopSeed();
        //    }

        //    return acc;
        //}

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