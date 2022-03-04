using JmesPath.Net.Utils;
using System.Text.Json.Nodes;

namespace JmesPath.Net.Expressions
{
    public sealed class Identifier : Expression
    {
        public Identifier(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        protected override Argument Transform(JsonNode json)
        {
            JsonNode node = JsonNodes.Null;

            if (json is JsonObject jsonObject && jsonObject.TryGetPropertyValue(Name, out var node_))
                node = node_ ?? node;

            return node;
        }
    }
}
