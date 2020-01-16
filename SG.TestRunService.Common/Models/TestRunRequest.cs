using SG.TestRunService.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunRequest
    {
        public int TestId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        Dictionary<string, ExtraDataValue> ExtraData { get; set; }

        public TestRun ToDataModel(int testRunSessionId)
        {
            return new TestRun()
            {
                TestRunSessionId = testRunSessionId,
                TestId = TestId,
                Outcome = Outcome,
                StartTime = StartTime,
                FinishTime = FinishTime,
                ExtraData = ExtraData
                    .Select(
                        e => new Data.ExtraData()
                        {
                            Name = e.Key,
                            Value = e.Value.Value,
                            Url = e.Value.Url
                        }).ToList()
            };
        }
    }
}
