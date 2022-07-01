using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathLiteral : JmesPathExpression
    {
        private readonly JToken value_;

        public override string ExpressionType
            => "literal";

        public JmesPathLiteral(JToken value)
        {
            value_ = value;
        }

        public JToken Value => value_;

        protected override JmesPathArgument Transform(JToken json)
        {
            return value_;
        }
    }
}