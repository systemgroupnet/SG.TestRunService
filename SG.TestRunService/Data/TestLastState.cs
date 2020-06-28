using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SG.TestRunService.Data
{
    public class TestLastState : IEntity<int>
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        public TestRunOutcome LastOutcome { get; set; }
        public int LastOutcomeProductBuildInfoId { get; set; }
        public DateTime LastOutcomeDate { get; set; }
        public int? LastImpactedProductBuildInfoId { get; set; }
        public DateTime? LastImpactedDate { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public TestCase TestCase { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public BuildInfo LastOutcomeProductBuildInfo { get; set; }

        [OnDelete(DeleteBehavior.Restrict)]
        public BuildInfo LastImpactedProductBuildInfo { get; set; }

        public bool IsImpactedAfter(DateTime dateTimeToCompare)
        {
            return ShouldBeRun && LastImpactedDate > dateTimeToCompare;
        }

        public class IDEqulityComparer : IEqualityComparer<TestLastState>
        {
            public bool Equals([AllowNull] TestLastState x, [AllowNull] TestLastState y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Id == y.Id;
            }

            public int GetHashCode([DisallowNull] TestLastState obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
