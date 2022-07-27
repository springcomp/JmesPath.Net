using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;
using System;

namespace DevLab.JmesPath.Functions
{
    public sealed class ReduceFunction : JmesPathFunction
    {
        public ReduceFunction(IScopeParticipant scopes)
            : base("reduce", 3, scopes)
        {
        }

        public override void Validate(params JmesPathFunctionArgument[] args)
        {
            System.Diagnostics.Debug.Assert(base.Scopes != null);

            EnsureArray(args[0]);
            EnsureExpressionType(args[2]);

            if (args[1].IsExpressionType)
                throw new Exception($"Error: invalid-type, unexpected expression type as the second parameter to the function {Name}.");

            if (!(args[2].Expression is JmesPathLambdaExpression lambda))
                throw new Exception($"Error: invalid-type, function {Name} expected a lambda-expression as its third parameter.");

            if (lambda.Arguments.Length != 2)
                throw new Exception($"Error: syntax, function {Name} expected a lambda-expression with exactly two arguments as its third parameters.");
        }

        public override JToken Execute(params JmesPathFunctionArgument[] args)
        {
            var acc = args[1].Token;
            var array = (JArray)args[0].Token;

            if (array.Count == 0)
                return acc;

            var lambda = args[2].Expression as JmesPathLambdaExpression;

            foreach (var cur in array)
            {
                try
                {
                    // { acc: acc, cur: cur }

                    var scope = new JObject();
                    scope.Add(lambda.Arguments[0], acc);
                    scope.Add(lambda.Arguments[1], cur);

                    scopes_?.PushScope(scope);

                    // evaluate lambda expression

                    var result = lambda.Transform(JTokens.Null);

                    // accumulate result

                    acc = result.Token;
                }
                finally {
                    scopes_?.PopScope();
                }
            }
            return acc;
        }
    }
}