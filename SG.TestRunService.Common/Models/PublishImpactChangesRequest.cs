using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class PublishImpactChangesRequest
    {
        public int AzureProductBuildDefinitionId { get; set; }
        public IEnumerable<string> CodeSignatures { get; set; }
        public bool NoBaseBuild { get; set; }
        public int? TestRunSessionId { get; set; }
        public int? AzureProductBuildId { get; set; }
    }
}
