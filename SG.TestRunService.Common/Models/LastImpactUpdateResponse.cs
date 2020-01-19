using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class LastImpactUpdateResponse
    {
        public int AzureProductBuildDefinitionId { get; set; }

        public DateTime UpdateDate { get; set; }
        public BuildInfo ProductBuild { get; set; }
        public TestRunSessionResponse TestRunSession { get; set; }
    }
}
