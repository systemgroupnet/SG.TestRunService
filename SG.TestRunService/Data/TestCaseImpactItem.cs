using SG.TestRunService.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestCaseImpactItem : IEntity
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        [Required]
        public int CodeSignatureId { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public bool IsDeleted { get; set; } = false;

        [OnDelete(DeleteBehavior.Cascade)]
        public TestCase TestCase { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public CodeSignature CodeSignature { get; set; }
    }
}
