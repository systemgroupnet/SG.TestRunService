using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class BuildInfo
    {
        [Required]
        public string TeamProject { get; set; }
        public int AzureBuildDefinitionId { get; set; }
        public int AzureBuildId { get; set; }
        [Required]
        public string SourceVersion { get; set; }
        public DateTime Date { get; set; }
        public string BuildNumber { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
