using JmesPath.Net.Expressions;
using JmesPath.Net.Utils;
using System.Text.Json.Nodes;
using Xunit;

namespace jmespath.tests;

public class IdentifierTests
{
    [Theory]
    [InlineData("foo", "{\"foo\": \"value\"}", "\"value\"")]
    [InlineData("bar", "{\"foo\": \"value\"}", "null")]
    [InlineData("foo", "{\"foo\": [0, 1, 2]}", "[0,1,2]")]
    [InlineData("with space", "{\"with space\": \"value\"}", "\"value\"")]
    [InlineData("special chars: !@#", "{\"special chars: !@#\": \"value\"}", "\"value\"")]
    [InlineData("quote\"char", "{\"quote\\\"char\": \"value\"}", "\"value\"")]
    [InlineData("\u2713", "{\"\u2713\": \"value\"}", "\"value\"")]
    public void Identifier(string identifier, string json, string expected)
    {
        var ast = new Identifier(identifier);
        var node = JsonNode.Parse(json);

        // system under test

        var argument = new Argument(node!);
        var result = ast.Transform(argument);

        Assert.NotNull(result);
        Assert.Equal(expected, result!.Node!.AsString());
    }
}