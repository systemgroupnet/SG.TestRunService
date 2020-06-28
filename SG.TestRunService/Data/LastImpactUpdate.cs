using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class LastImpactUpdate : IEntity<int>
    {
        public int Id { get; set; }

        public int AzureProductBuildDefinitionId { get; set; }

        public DateTime UpdateDate { get; set; }
        public int ProductBuildInfoId { get; set; }
        public int? TestRunSessionId { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public BuildInfo ProductBuildInfo { get; set; }
        [OnDelete(DeleteBehavior.Restrict)]
        public TestRunSession TestRunSession { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
