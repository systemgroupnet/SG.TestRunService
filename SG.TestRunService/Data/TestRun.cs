using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestRun : IEntity
    {
        public int Id { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunSession Session { get; set; }
        public int TestCaseId { get; set; }
        public TestCase TestCase { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
