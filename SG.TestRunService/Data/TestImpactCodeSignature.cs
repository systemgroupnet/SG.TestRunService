using SG.TestRunService.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestImpactCodeSignature : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string Signature { get; set; }
        [Required]
        public int TestCaseId { get; set; }
        [Required]
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
        public bool IsDelelted { get; set; }
    }
}
