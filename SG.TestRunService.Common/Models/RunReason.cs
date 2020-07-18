namespace SG.TestRunService.Common.Models
{
    public enum RunReason
    {
        Impacted = 1,
        Failed = 2,
        ForceRun = 3,
        ImpactDataNotAvailable = 4,
        NoBaseBuild = 5,
        Aborted = 6,
        ImpactUpdateFailed = 7,
        New = 10
    }
}
