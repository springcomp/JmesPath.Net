using System.Collections.Generic;
using DevLab.JmesPath.Interop;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathMultiSelectHash : JmesPathExpression
    {
        private readonly IDictionary<string, JmesPathExpression> dictionary_
            = new Dictionary<string, JmesPathExpression>()
            ;

        public JmesPathMultiSelectHash(IDictionary<string, JmesPathExpression> dictionary)
        {
            foreach (var key in dictionary.Keys)
                dictionary_.Add(key, dictionary[key]);
        }

        protected override JmesPathArgument OnTransform(JmesPathArgument json)
        {
            var properties = new List<JProperty>();

            foreach (var key in dictionary_.Keys)
            {
                var expression = dictionary_[key];
                var result = expression.Transform(json).AsJToken();
                properties.Add(new JProperty(key, result));
            }

            return new JObject(properties);
        }

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);
            foreach (var key in dictionary_.Keys)
                dictionary_[key].Accept(visitor);
        }
    }
}