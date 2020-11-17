using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathNotExpression : JmesPathSimpleExpression
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="JmesPathNotExpression"/>
        /// that negates the result of evaluating its associated expression.
        /// </summary>
        /// <param name="expression"></param>
        public JmesPathNotExpression(JmesPathExpression expression)
            : base(expression)
        {
        }

        protected override JmesPathArgument OnTransform(JmesPathArgument json)
        {
            var token = base.OnTransform(json.Token);
            return JmesPathArgument.IsFalse(token) 
                ? JmesPathArgument.True
                : JmesPathArgument.False
                ;
        }
    }
}