using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class ProductLine : IEntity
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int? AzureProductBuildDefinitionId { get; set; }
    }
}
