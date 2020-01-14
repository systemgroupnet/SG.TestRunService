using SG.TestRunService.DbServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestRun : IEntity
    {
        public int Id { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunSession Session { get; set; }
        public int TestId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public IList<Attachement> Attachements { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }

    public enum TestRunOutcome
    {
        NotStarted = 0,
        FixtureQueued = 1,
        WaitingForWeb = 2,
        Running = 3,
        Succeeded = 16,
        Failed = 17,
        Unknown = 128
    }
}
