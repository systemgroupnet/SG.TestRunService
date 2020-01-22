namespace SG.TestRunService.Common.Models
{
    public enum TestRunSessionState
    {
        NotStarted = 0,
        Running = 1,
        RanToEnd = 2,
        Cancelled = 3,
        FatalError = 4,
        Unknown = 128
    }
}
