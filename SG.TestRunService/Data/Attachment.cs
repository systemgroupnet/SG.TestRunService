using SG.TestRunService.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class Attachment : IEntity
    {
        public int Id { get; set; }

        public int? TestRunId { get; set; }
        public int? TestRunSessionId { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
