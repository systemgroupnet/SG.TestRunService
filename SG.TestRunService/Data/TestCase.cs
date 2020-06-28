using SG.TestRunService.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestCase : IEntity<int>
    {
        public TestCase()
        {
            ExtraData = new List<ExtraData>();
        }

        public int Id { get; set; }
        public int AzureTestCaseId { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string Title { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
