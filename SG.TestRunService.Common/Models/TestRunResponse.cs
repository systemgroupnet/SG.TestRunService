using SG.TestRunService.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Common.Models
{
    public class TestRunResponse
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }

        public static TestRunResponse From(TestRun testRun)
        {
            return new TestRunResponse()
            {
                Id = testRun.Id,
                TestId = testRun.TestId,
                TestRunSessionId = testRun.TestRunSessionId,
                Outcome = testRun.Outcome,
                StartTime = testRun.StartTime,
                FinishTime = testRun.FinishTime
            };
        }

        public static IQueryable<TestRunResponse> Project(IQueryable<TestRun> runs)
        {
            return runs.Select(
                x => new TestRunResponse()
                {
                    Id = x.Id,
                    TestId = x.TestId,
                    TestRunSessionId = x.TestRunSessionId,
                    Outcome = x.Outcome,
                    StartTime = x.StartTime,
                    FinishTime = x.FinishTime
                });
        }
    }
}
