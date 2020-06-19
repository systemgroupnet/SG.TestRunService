using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestLastStateResponse
    {
        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        public TestRunOutcome LastOutcome { get; set; }
        public BuildInfo LastOutcomeProductBuildInfo { get; set; }
        public DateTime LastOutcomeDate { get; set; }
        public BuildInfo LastImpactedProductBuildInfo { get; set; }
        public DateTime? LastImpactedDate { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }
    }
}
