using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class ProductLine : ProductLineIdOrKey
    {
        public int? AzureProductBuildDefinitionId { get; set; }
    }
}
