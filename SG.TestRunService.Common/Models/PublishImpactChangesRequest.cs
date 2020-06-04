using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class PublishImpactChangesRequest
    {
        public int AzureProductBuildDefinitionId { get; set; }
        [Obsolete("Use `CodeSignatures` property.")]
        public IList<CodeSignature> Changes { get; set; }
        public IList<string> CodeSignatures { get; set; }
        public bool RunAllTests { get; set; }
        public int? TestRunSessionId { get; set; }
        public int? AzureProductBuildId { get; set; }
    }
}
