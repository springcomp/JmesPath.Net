using System;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathCurrentNodeExpression : JmesPathExpression
    {
        public override string ExpressionType
            => "current-node";

        protected override JmesPathArgument Transform(JToken json)
        {
            return json;
        }
    }
}