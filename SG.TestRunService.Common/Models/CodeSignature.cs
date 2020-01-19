using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class CodeSignature
    {
        public CodeSignature() {}
        public CodeSignature(string fileName, string signature)
        {
            FileName = fileName;
            Signature = signature;
        }

        public string FileName { get; set; }
        public string Signature { get; set; }
    }
}
