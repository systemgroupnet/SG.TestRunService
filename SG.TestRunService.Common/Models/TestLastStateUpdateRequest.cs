using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestLastStateUpdateRequest
    {
        public int AzureProductBuildDefinitionId { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public RunReason? DictatedRunReason { get; set; }
    }
}
