using SG.TestRunService.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestCase : IEntity
    {
        public int Id { get; set; }
        public int Azure_TestCaseId { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string Title { get; set; }
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
