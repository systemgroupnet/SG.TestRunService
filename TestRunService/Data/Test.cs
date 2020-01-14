using SG.TestRunService.DbServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SG.TestRunService.Data
{
    public class Test : IEntity
    {
        public int Id { get; set; }
        public int Azure_TestCaseId { get; set; }
        public int Azure_TestBuildId { get; set; }
        public string Title { get; set; }
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
