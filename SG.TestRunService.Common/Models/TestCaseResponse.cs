using System.Collections.Generic;

namespace SG.TestRunService.Common.Models
{
    public class TestCaseResponse : IExtraDataContainer
    {
        public TestCaseResponse()
        {
            ExtraData = new Dictionary<string, ExtraDataValue>();
        }

        public int Id { get; set; }
        public int AzureTestCaseId { get; set; }
        public string TeamProject { get; set; }
        public string Title { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}