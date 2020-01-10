using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestRunSession
    {
        public int Id { get; set; }
        public string TeamProject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int Azure_ProductBuildId { get; set; }
        public int Azure_TestBuildId { get; set; }
        public string Azure_ProductBuildNumber { get; set; }
        public string Azure_TestBuildNumber { get; set; }
        public string SourceVersion { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public IList<TestRun> TestRuns { get; set; }
        public IList<Attachement> Attachements { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }

    public enum TestSessionOutcome
    {
        NotStarted = 0,
        Running = 1,
        Succeeded = 16,
        Failed = 17,
        Unknown = 128
    }
}
