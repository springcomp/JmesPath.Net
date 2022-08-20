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
            => argument.IsProjection
                ? Project(argument.Projection)
                : Transform(argument.Token)
                ;

        protected virtual JmesPathArgument Project(IEnumerable<JmesPathArgument> arguments)
        {
            var items = new List<JmesPathArgument>();
            foreach (var projected in arguments)
            {
                var item = Transform(projected);
                if (item.IsProjection || item.Token != JTokens.Null)
                    items.Add(item);
            }

            return new JmesPathArgument(items);
        }

        private JmesPathArgument Reduce(IEnumerable<JmesPathArgument> arguments)
        {
            System.Diagnostics.Debug.Assert(accumulator_ != null);
            // accumulator_.PushSeed() has been called by JmesPathReduceProjection

            try
            {
                foreach (var argument in arguments)
                {
                    var item = Transform(argument);
                    accumulator_.Accumulator = item.Token;
                }

                return accumulator_.Accumulator;
            }
            finally
            {
                accumulator_.PopSeed();
            }
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
