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

        public override string ToString()
        {
            return
                $"Team Project: {TeamProject}\r\n" +
                $"Azure Build Definition Id: {AzureBuildDefinitionId}\r\n" +
                $"Azure Build Id: {AzureBuildId}\r\n" +
                $"Build Number: {BuildNumber}\r\n" +
                $"Source Version: {SourceVersion}\r\n" +
                $"Date: {Date.ToString(Helpers.DTFormat)}\r\n";
        }
    }
}
