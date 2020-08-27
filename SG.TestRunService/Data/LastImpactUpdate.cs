using SG.TestRunService.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class LastImpactUpdate : IEntity<int>
    {
        public int Id { get; set; }

        public int ProductLineId { get; set; }

        public DateTime UpdateDate { get; set; }
        public int ProductBuildInfoId { get; set; }
        public int? TestRunSessionId { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public BuildInfo ProductBuildInfo { get; set; }
        [OnDelete(DeleteBehavior.Restrict)]
        public TestRunSession TestRunSession { get; set; }
        [OnDelete(DeleteBehavior.Restrict)]
        public ProductLine ProductLine { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
