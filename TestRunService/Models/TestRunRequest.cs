using SG.TestRunService.Data;
using System;

namespace SG.TestRunService.Models
{
    public class TestRunRequest
    {
        public int TestId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
    }
}
