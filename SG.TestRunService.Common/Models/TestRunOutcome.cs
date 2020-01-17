namespace SG.TestRunService.Common.Models
{
    public enum TestRunOutcome
    {
        NotStarted = 0,
        FixtureQueued = 1,
        WaitingForWeb = 2,
        Running = 3,
        Succeeded = 16,
        Failed = 17,
        Unknown = 128
    }
}
