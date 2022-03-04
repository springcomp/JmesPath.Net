using JmesPath.Net.Interop;
using System.Text.Json.Nodes;

namespace JmesPath.Net.Expressions
{
    public abstract class Expression
    {
        /// <summary>
        /// Evaluates the expression against the specified JSON object.
        /// The result cannot be null and is:
        /// either a valid JSON found in the resulting <see cref="JmesPathArgument"/>'s Token property.
        /// or a projection found in the resulting <see cref="JmesPathArgument"/>'s Projection property.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public virtual Argument Transform(Argument argument)
        {
            if (argument.IsProjection)
            {
                var items = new List<Argument>();
                foreach (var projected in argument.AsProjection())
                {
                    var item = Transform(projected);
                    if (item.IsProjection)
                        items.Add(item);
                    else if (item.IsNull)
                        items.Add(item);
                }

                return new Argument(items);
            }

            return Transform(argument.AsNode());
        }

        /// <summary>
        /// Evaluates the expression against the specified JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected abstract Argument Transform(JsonNode json);

        public bool IsExpressionType { get; private set; }

        public static void MakeExpressionType(Expression expression)
        {
            expression.IsExpressionType = true;
        }

        /// <summary>
        /// Perform a traversal of the abstract syntax tree.
        /// </summary>
        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
