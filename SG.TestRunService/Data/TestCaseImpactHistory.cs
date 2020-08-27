using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class TestCaseImpactHistory : IEntity
    {
        public int TestCaseId { get; set; }
        public int ProductLineId { get; set; }
        public int CodeSignatureId { get; set; }
        public DateTime Date { get; set; }
        public int ProductBuildInfoId { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public TestCase TestCase { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public CodeSignature CodeSignature { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public BuildInfo ProductBuildInfo { get; set; }
    }
}
