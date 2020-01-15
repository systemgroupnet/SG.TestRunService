using SG.TestRunService.Common.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Data
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
