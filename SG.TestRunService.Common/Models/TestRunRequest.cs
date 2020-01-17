using System;
using System.Collections.Generic;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunRequest
    {
        public int TestCaseId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public Dictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
