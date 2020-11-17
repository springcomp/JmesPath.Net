using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DevLab.JmesPath.Expressions
{
    public abstract class JmesPathProjection : JmesPathExpression
    {
        public abstract JmesPathArgument Project(JmesPathArgument argument);

        protected override JmesPathArgument OnTransform(JmesPathArgument json)
        {

           return Project(json);

      
        }
    }
}