using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestCaseRequest
    {
        public int Azure_TestCaseId { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string Title { get; set; }
        public Dictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
