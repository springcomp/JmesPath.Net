using System.Collections.Generic;
using System.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathLambdaExpression : JmesPathSimpleExpression
    {
        private readonly string[] args_;

        /// <summary>
        /// Initialize a new instance of the <see cref="JmesPathLambdaExpression" /> class.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="args"></param>
        public JmesPathLambdaExpression(JmesPathExpression expression, IEnumerable<string> args)
            : base(expression)
        {
            args_ = args.ToArray();
        }

        public string[] Arguments => args_;
    }
}
