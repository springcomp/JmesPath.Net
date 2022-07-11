
using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Utils;
using JmesPath.Net.Expressions;
using JmesPath.Net.Utils;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;

var jj = new JmesPath.Net.JmesPath();
var jo = new DevLab.JmesPath.JmesPath();

const string json = "{\"foo\": \"bar\"}";
var nsj = JToken.Parse(json);
var stj = JsonNode.Parse(json);

const int COUNT = 100000;

var identifier = new Identifier("foo");
var sw = Stopwatch.StartNew();
for (var index = 0; index < COUNT; index++)
    identifier.Transform(stj);
sw.Stop();
Console.WriteLine($"{COUNT} iterations completed in {sw.Elapsed.TotalMilliseconds}ms.");

var jid = new JmesPathIdentifier("foo");
sw = Stopwatch.StartNew();
for (var index = 0; index < COUNT; index++)
    jid.Transform(nsj);
sw.Stop();
Console.WriteLine($"{COUNT} iterations completed in {sw.Elapsed.TotalMilliseconds}ms.");
