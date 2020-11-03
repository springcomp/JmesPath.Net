using System;
using System.Collections.Generic;
using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Functions
{
    public abstract class JmesPathContextFunction : JmesPathFunction
    {
        private readonly IDictionary<string, JmesPathArgument> context_;
        protected JmesPathContextFunction(string name, IDictionary<string, JmesPathArgument> context, int count)
            : base(name, count)
        {
            context_ = context;
        }

        protected IDictionary<string, JmesPathArgument> Context
            => context_;
    }

    public sealed class EvaluateExpressionFunction : JmesPathContextFunction
    {
        public EvaluateExpressionFunction(IDictionary<string, JmesPathArgument> context)
            : base("evaluate", context, 1)
        {
        }

        public override JToken Execute(params JmesPathFunctionArgument[] args)
        {
            System.Diagnostics.Debug.Assert(args.Length == 1);
            System.Diagnostics.Debug.Assert(args[0].IsToken);

            var name = args[0].Token.Value<string>();

            var result = Context[name];

            return JTokens.Null;
        }
    }
}