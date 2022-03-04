using JmesPath.Net.Utils;
using System.Text.Json.Nodes;

namespace JmesPath.Net.Expressions
{
    public sealed class Argument
    {
        public Argument(JsonNode node)
        {
            Node = node;
            Projection = null;
        }

        public Argument(IEnumerable<Argument> items)
        {
            System.Diagnostics.Debug.Assert(items != null);
            Node = JsonNodes.Null;
            Projection = items.ToArray();
        }

        public JsonNode? Node { get; }

        public Argument[]? Projection { get; }

        public bool IsNull
            => Node != null && Node == JsonNodes.Null;

        public bool IsProjection
            => Projection != null;

        public static implicit operator Argument(JsonNode node)
            => new Argument(node);

        public JsonNode AsNode()
        {
            if (Node != null)
                return Node;

            var items = new List<JsonNode>();
            foreach (var projected in Projection!)
                items.Add(projected.AsNode());

            return new JsonArray(items.ToArray());
        }

        public Argument[] AsProjection()
        {
            if (Projection != null)
                return Projection;

            System.Diagnostics.Debug.Assert(false);
            return new Argument[] { };
        }
    }
}
