using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathReduceExpression : JmesPathExpression
    {
        private readonly JmesPathExpression seed_;
        private readonly JmesPathExpression reduce_;

        public JmesPathReduceExpression(JmesPathExpression seed, JmesPathExpression reduce)
        {
            seed_ = seed;
            reduce_ = reduce;
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            var array = json as JArray;
            if (array == null || array.Count == 0)
                return JTokens.Null;

            var acc = seed_?.Transform(json).AsJToken() ?? JTokens.Null;

            try
            {
                accumulator_.PushSeed(acc);
                foreach (var element in array)
                {
                    var result = reduce_.Transform(element).AsJToken();
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

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);
            seed_?.Accept(visitor);
            reduce_.Accept(visitor);
        }
    }
}