namespace SG.TestRunService.Common.Models
{
    public enum TestSessionOutcome
    {
        NotStarted = 0,
        Running = 1,
        Succeeded = 16,
        Failed = 17,
        Unknown = 128
    }
}
