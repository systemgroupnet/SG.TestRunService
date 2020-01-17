using System;
using System.Collections.Generic;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunRequest
    {
        public TestRunRequest()
        {
            ExtraData = new Dictionary<string, ExtraDataValue>();
        }

        public int TestCaseId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
