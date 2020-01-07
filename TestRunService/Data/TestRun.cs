using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestRun
    {
        public int Id { get; set; }
        public int TestRunId { get; set; }
        public TestRunSession Run { get; set; }
        public int Azure_TestCaseId { get; set; }
        //public long Azure_
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
