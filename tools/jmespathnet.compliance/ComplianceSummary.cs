namespace jmespath.net.compliance
{
#if NET6_0
    public record ComplianceSummary(double Percent, int Succeeded, int Total, int Failed);
#else
    public sealed class ComplianceSummary
    {
        public ComplianceSummary(double percent, int succeeded, int total, int failed)
        {
            Percent = percent;
            Succeeded = succeeded;
            Total = total;
            Failed = failed;
        }

        public double Percent { get; }
        public int Succeeded { get; }
        public int Total { get; }
        public int Failed { get; }
    }
#endif
}