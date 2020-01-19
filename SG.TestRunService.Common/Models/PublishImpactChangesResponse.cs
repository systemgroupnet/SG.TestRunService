using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class PublishImpactChangesResponse
    {
        public IReadOnlyList<TestToRunResponse> TestsToRun { get; set; }
        public int TestsToRunCount { get; set; }
    }
}
