using AutoMapper;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Models
{
    [AutoMap(typeof(TestRunSession))]
    public class TestRunSessionRequest
    {
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        [Required]
        public int Azure_ProductBuildId { get; set; }
        [Required]
        public int Azure_TestBuildId { get; set; }
        [Required]
        public string Azure_ProductBuildNumber { get; set; }
        [Required]
        public string Azure_TestBuildNumber { get; set; }
        [Required]
        public string SourceVersion { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public List<TestRunRequest> TestRuns { get; set; }
    }
}
