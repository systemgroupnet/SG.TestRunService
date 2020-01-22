namespace SG.TestRunService.Common.Models
{
    public enum TestRunOutcome
    {
        None = 0,
        Successful = 1,
        Failed = 2,
        Aborted = 3,
        FatalError = 10,
        Unknown = 128
    }
}
