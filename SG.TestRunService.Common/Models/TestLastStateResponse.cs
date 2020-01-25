using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestLastStateResponse
    {
        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        public BuildInfo ProductBuildInfo { get; set; }
        public DateTime UpdateDate { get; set; }
        public TestRunOutcome? LastOutcome { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }
    }
}
