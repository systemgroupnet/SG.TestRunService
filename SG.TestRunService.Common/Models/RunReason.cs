namespace SG.TestRunService.Common.Models
{
    public enum RunReason
    {
        Impacted = 1,
        Failed = 2,
        ForceRun = 3,
        ImpactDataNotAvailable = 4,
        NoBaseBuild = 5,
        New = 10
    }
}
