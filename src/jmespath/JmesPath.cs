using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using DevLab.JmesPath;

using JmesPath.Net.Expressions;
using JmesPath.Net.Interop;
using JmesPath.Net.Utils;

namespace JmesPath.Net
{
    using JmesPathExpression = Expressions.Expression;

    public sealed class JmesPath
    {
        private readonly Encoding _encoding;

        public JmesPath() : this(Encoding.UTF8)
        {
        }

        public JmesPath(Encoding encoding)
        {
            _encoding = encoding;
        }
        public String Transform(string json, string expression)
        {
            var document = ParseJson(json);
            var jmespath = Parse(expression);
            var result = jmespath.Transform(document);

            return result.ToJsonString();
        }

        public sealed class Expression : JmesPathExpression
        {
            private readonly JmesPathExpression expression_;

            internal Expression(JmesPathExpression expression)
            {
                expression_ = expression;
            }

            public string Transform(string document)
            {
                var token = ParseJson(document);
                var result = Transform(token);
                return result.ToString();
            }

            public JsonNode Transform(JsonDocument document)
            {
                var node = JsonNodes.Null;
                var rootElement = document.RootElement;
                switch (rootElement.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = JsonObject.Create(rootElement);
                        break;

                    case JsonValueKind.Array:
                        node = JsonArray.Create(rootElement);
                        break;

                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        node = JsonValue.Create(rootElement);
                        break;

                    case JsonValueKind.Null:
                        return JsonNodes.Null;

                    case JsonValueKind.Undefined:
                        System.Diagnostics.Debug.Assert(false);
                        throw new NotSupportedException();
                }

                return Transform(node!).AsNode();
            }

            protected override Argument Transform(JsonNode json)
                => expression_.Transform(json);
        }

        public Expression Parse(string expression)
        {
            return Parse(new MemoryStream(_encoding.GetBytes(expression)));
        }

        public Expression Parse(Stream stream)
        {
            var analyzer = new AstBuilder();
            Parser.Parse(stream, _encoding, analyzer);

            // perform post-parsing syntax validation

            var syntax = new SyntaxVisitor();
            analyzer.Expression.Accept(syntax);

            return new Expression(analyzer.Expression);
        }

        public static JsonDocument ParseJson(string input)
        {
            try
            {
                return JsonDocument.Parse(input);
            }
            catch (Exception e)
            { 
                throw new Exception("Unable to read the JSON string.");
            }
        }

        private sealed class SyntaxVisitor : IVisitor
        {
            public void Visit(JmesPathExpression expression)
            {
                //var projection = expression as JmesPathSliceProjection;
                //if (projection?.Step != null && projection.Step.Value == 0)
                //    throw new Exception("Error: invalid-value, a slice projection step cannot be 0.");
            }
        }
    }
}
