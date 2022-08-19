using System.Collections.Generic;
using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    /// <summary>
    /// Represents the base class for a JmesPath expression.
    /// </summary>
    public abstract class JmesPathExpression
    {
        protected IReduceAccumulator accumulator_ = null;

        /// <summary>
        /// Evaluates the expression against the specified JSON object.
        /// The result cannot be null and is:
        /// either a valid JSON found in the resulting <see cref="JmesPathArgument"/>'s Token property.
        /// or a projection found in the resulting <see cref="JmesPathArgument"/>'s Projection property.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public virtual JmesPathArgument Transform(JmesPathArgument argument)
        {
            if (argument.IsProjection)
            {
                var isReduction = argument.IsReduction;

                try
                {
                    var items = new List<JmesPathArgument>();
                    foreach (var projected in argument.Projection)
                    {
                        var item = Transform(projected);

                        if (isReduction)
                            accumulator_.Accumulator = item.Token;

                        else if (item.IsProjection || item.Token != JTokens.Null)
                            items.Add(item);
                    }

                    return isReduction
                        ? accumulator_.Accumulator
                        : new JmesPathArgument(items)
                        ;
                }
                finally
                {
                    if (isReduction)
                        accumulator_.PopSeed();
                }
            }

            return Transform(argument.Token);
        }

        /// <summary>
        /// Evaluates the expression against the specified JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected abstract JmesPathArgument Transform(JToken json);

        internal void SetAccumulator(IReduceAccumulator accumulator)
            => accumulator_ = accumulator;

        public bool IsExpressionType { get; private set; }

        public static void MakeExpressionType(JmesPathExpression expression)
            => expression.IsExpressionType = true;

        /// <summary>
        /// Perform a traversal of the abstract syntax tree.
        /// </summary>
        public virtual void Accept(IVisitor visitor)
            => visitor.Visit(this);

        public override string ToString()
            => $"{(IsExpressionType ? "&" : "")}{Format()}";

        protected abstract string Format();
    }
}
