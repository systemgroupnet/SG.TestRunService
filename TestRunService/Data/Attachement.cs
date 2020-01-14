using SG.TestRunService.DbServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class Attachement : IEntity
    {
        public int Id { get; set; }
        public int? TestRunId { get; set; }
        public int? TestCaseRunId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
