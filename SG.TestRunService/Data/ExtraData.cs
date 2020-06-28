using SG.TestRunService.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class ExtraData : IEntity<int>
    {
        public int Id { get; set; }
        public int? TestCaseId { get; set; }
        public int? TestRunId { get; set; }
        public int? TestRunSessionId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
        public string Url { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
