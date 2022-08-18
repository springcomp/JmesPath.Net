using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathReduceProjection : JmesPathCompoundExpression
    {
        public JmesPathReduceProjection(JmesPathExpression seed, JmesPathExpression reduction)
            : base(seed, reduction)
        {
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            var array = json as JArray;
            if (array == null || array.Count == 0)
                return JTokens.Null;

            var acc = Left?.Transform(json).AsJToken() ?? JTokens.Null;

            try
            {
                accumulator_.PushSeed(acc);
                foreach (var element in array)
                {
                    var result = Right.Transform(element).AsJToken();
                    accumulator_.Accumulator = result;
                }
                acc = accumulator_.Accumulator;
            }
            finally
            {
                accumulator_.PopSeed();
            }

            return acc;
        }

        protected override string Format()
            => $"[?{Left}].{Right}";
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