using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SG.TestRunService.Data
{
    public class ExtraData
    {
        public int Id { get; set; }

        public int? TestId { get; set; }
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
