using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class CodeSignature : IEntity
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Signature { get; set; }
        public string Path { get; set; }
        public CodeSignatureType Type { get; set; }
    }
}
