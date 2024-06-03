using System.Text;

using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Attributes;

using jmespath.lexer;
using DevLab.JmesPath;

BenchmarkSwitcher.FromAssembly(typeof(Tests).Assembly).Run(args);

[ShortRunJob]
[MemoryDiagnoser]
public class Tests
{
	private static Stream expression_
		= new MemoryStream();

	[Benchmark] public void RunNew() { for (var i = 0; i < 10_000; i++) ScanNew(); }
	[Benchmark] public void Run() { for (var i = 0; i < 10_000; i++) Scan(); }
	private const string expression = "foo";

	public void ScanNew()
	{
		var scanner = new Scanner(expression);
		Token token;
		do
		{
			token = scanner.GetNextToken();
			if (token.Type == TokenType.EOF)
				break;

		} while (true);
	}

	public void Scan()
	{
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(expression));
		var scanner = new JmesPathScanner(stream, Encoding.UTF8);
		scanner.InitializeLookaheadQueue();

		int token;
		while ((token = scanner.yylex()) != TokenType.EOF)
			;
	}
}