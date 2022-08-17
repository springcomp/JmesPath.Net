using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Interop;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DevLab.JmesPath.Functions
{
    public class ReduceFunction : JmesPathFunction
    {
        private JArray array_;
        private JObject seed_;
        private JmesPathExpression expression_;

        public ReduceFunction(IScopeParticipant scopes)
            : base("reduce", 3, scopes)
        {
        }

        public override void Validate(params JmesPathFunctionArgument[] args)
        {
            System.Diagnostics.Debug.Assert(base.Scopes != null);

            array_ = EnsureArray(args[0]);
            seed_ = EnsureObject(args[1]);
            expression_ = EnsureExpressionType(args[2]);
        }

        public override JToken Execute(params JmesPathFunctionArgument[] args)
        {
            var accumulator = seed_.Properties().First();
            var name = accumulator.Name;
            var acc = accumulator.Value;

            if (array_.Count == 0)
                return acc;

            foreach (var cur in array_)
            {
                try
                {
                    // { acc: acc }

                    var scope = new JObject();
                    scope.Add(name, acc);
                    scopes_?.PushScope(scope);

                    // evaluate expression

                    var result = expression_.Transform(cur);

                    // accumulate result

                    acc = result.Token;
                }
                catch
                {
                    scopes_?.PopScope();
                }
            }

            return acc;
        }
    }
}