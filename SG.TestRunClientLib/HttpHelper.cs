using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public class HttpHelper
    {
        public static JsonPatchDocument<TModel> CreateJsonPatchToAddOrUpdateExtraData<TModel>(
            IDictionary<string, ExtraDataValue> extraData)
            where TModel : class, IExtraDataContainer
        {
            var patch = new JsonPatchDocument<TModel>();
            foreach(var keyValue in extraData)
            {
                patch.Operations.Add(
                    new Operation<TModel>()
                    {
                        op = "add",
                        path = $"/{nameof(IExtraDataContainer.ExtraData)}/{keyValue.Key}",
                        value = keyValue.Value
                    });
            }
            return patch;
        }
    }
}
