using System;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Functions
{
    public abstract class JmesPathContextFunction : JmesPathFunction
    {
        protected JmesPathContextFunction(string name, int count)
            : base(name, count)
        {
        }
    }

    public sealed class EvaluateExpressionFunction : JmesPathContextFunction
    {
        public EvaluateExpressionFunction()
            : base("evaluate", 1)
        {
        }

        public override JToken Execute(params JmesPathFunctionArgument[] args)
        {
            System.Diagnostics.Debug.Assert(args.Length == 1);
            System.Diagnostics.Debug.Assert(args[0].IsToken);

            var name = args[0].Token.Value<string>();

            return JTokens.Null;
        }
    }
}