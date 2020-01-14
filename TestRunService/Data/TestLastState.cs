using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class TestLastState : IEntity
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public int Azure_TestBuildId { get; set; }
        public int Azure_ProductBuildId { get; set; }
        public int UpdateDate { get; set; }
        public string SourceVersion { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public bool ShouldBeRun { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
