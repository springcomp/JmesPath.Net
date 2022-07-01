namespace DevLab.JmesPath
{
    public interface IJmesPathGenerator2 : IJmesPathGenerator
    {
        bool InFunctionArg { get; }

        string ExpressionType { get; }
    }
}
