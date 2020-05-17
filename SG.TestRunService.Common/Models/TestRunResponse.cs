using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Common.Models
{
    public class TestRunResponse
    {
        public int Id { get; set; }
        public int TestCaseId { get; set; }
        public TestCaseResponse TestCase { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunState State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
