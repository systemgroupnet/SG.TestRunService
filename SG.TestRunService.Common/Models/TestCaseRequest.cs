using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestCaseRequest
    {
        public int AzureTestCaseId { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string Title { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
