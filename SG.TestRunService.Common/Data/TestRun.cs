using SG.TestRunService.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Data
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
        public IList<Attachment> Attachments { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
