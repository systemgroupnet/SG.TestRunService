using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestRun : IEntity
    {
        public TestRun()
        {
            Attachments = new List<Attachment>();
            ExtraData = new List<ExtraData>();
        }

        public int Id { get; set; }
        public int TestRunSessionId { get; set; }
        public int TestCaseId { get; set; }
        public TestRunState State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public TestRunSession TestRunSession { get; set; }
        [OnDelete(DeleteBehavior.Restrict)]
        public TestCase TestCase { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public IList<Attachment> Attachments { get; set; }
        [OnDelete(DeleteBehavior.Cascade)]
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
