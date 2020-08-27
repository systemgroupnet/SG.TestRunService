using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class PublishImpactChangesRequest
    {
        public ProductLine ProductLine { get; set; }
        public IEnumerable<string> CodeSignatures { get; set; }
        public bool NoBaseBuild { get; set; }
        public int? TestRunSessionId { get; set; }
        public BuildInfo ProductBuild { get; set; }
    }
}
