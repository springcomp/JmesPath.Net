namespace JmesPath.Net.Utils
{
    public static class JsonNodeExtensions
    {
        public static string AsString(this JsonNode node)
            => node?.ToJsonString(new JsonSerializerOptions()) ?? "null";
    }
}
