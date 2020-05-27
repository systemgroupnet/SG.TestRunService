using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public interface IExtraDataContainer
    {
        IDictionary<string, ExtraDataValue> ExtraData { get; }
    }
}
