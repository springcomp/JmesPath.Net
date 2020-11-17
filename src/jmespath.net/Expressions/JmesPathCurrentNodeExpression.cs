using System;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathCurrentNodeExpression : JmesPathExpression
    {
        protected override JmesPathArgument OnTransform(JmesPathArgument json)
        {
            return json;
        }
    }
}