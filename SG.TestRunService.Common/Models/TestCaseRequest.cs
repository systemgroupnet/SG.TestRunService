using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestCaseRequest : IExtraDataContainer
    {
        public TestCaseRequest() { }

        public TestCaseRequest(string teamProject, int azureTestCaseId, string title)
        {
            TeamProject = teamProject;
            AzureTestCaseId = azureTestCaseId;
            Title = title;
            ExtraData = new Dictionary<string, ExtraDataValue>();
        }

        public int AzureTestCaseId { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string Title { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
