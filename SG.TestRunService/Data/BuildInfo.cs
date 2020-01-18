using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class BuildInfo : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string TeamProject { get; set; }
        public int AzureBuildDefinitionId { get; set; }
        public int AzureBuildId { get; set; }
        [Required]
        public string SourceVersion { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string BuildNumber { get; set; }
    }
}
