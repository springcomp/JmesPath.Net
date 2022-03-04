
var expression = "foo";
var text = "{\"foo\": \"bar\"}";
var counter = 50000;

var which = "newtonsoft-json";
var parse = "parse";

if (args.Length != 0 && args[0] == "--system-text-json")
    which = "system-text-json";

if (args.Length == 2 && args[1] == "--no-parse")
    parse = "no-parse";

if (args.Length == 3 && Int32.TryParse(args[2], out var n_))
    counter = n_;

Action<string, string, int> Run = 
     (which == "newtonsoft-json")
     ? ((parse == "no-parse") ? NewtonsoftJson : NewtonsoftJsonP)
     : ((parse == "no-parse") ? SystemTextJson : SystemTextJsonP)
     ;

Console.WriteLine(which);
Console.WriteLine(parse);
Console.WriteLine(counter);

Run(expression, text, counter);
Environment.Exit(0);

static void NewtonsoftJson(string expression, string text, int counter)
{
    var jp = new DevLab.JmesPath.JmesPath();
    var ast = jp.Parse(expression);
    var document = DevLab.JmesPath.JmesPath.ParseJson(text);

    for (var i = 0; i < counter; i++)
        ast.Transform(document);
}
static void NewtonsoftJsonP(string expression, string text, int counter)
{
    for (var i = 0; i < counter; i++)
    {
        var jp = new DevLab.JmesPath.JmesPath();
        var ast = jp.Parse(expression);
        var document = DevLab.JmesPath.JmesPath.ParseJson(text);
        ast.Transform(document);
    }
}

static void SystemTextJsonP(string expression, string text, int counter)
{
    for (var i = 0; i < counter; i++)
    {
        var jp = new JmesPath.Net.JmesPath();
        var ast = jp.Parse(expression);
        var document = JmesPath.Net.JmesPath.ParseJson(text);
        ast.Transform(document);
    }
}
static void SystemTextJson(string expression, string text, int counter)
{
    var jp = new JmesPath.Net.JmesPath();
    var ast = jp.Parse(expression);
    var document = JmesPath.Net.JmesPath.ParseJson(text);

    for (var i = 0; i < counter; i++)
        ast.Transform(document);
}
