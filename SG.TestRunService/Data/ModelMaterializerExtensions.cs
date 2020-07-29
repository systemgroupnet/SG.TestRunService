using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public static class ModelMaterializerExtensions
    {
        public static async Task<List<TestRunSessionResponse>> MaterializeAllAsync(this IQueryable<TestRunSession> sessions)
        {
            var list = await sessions.Project().ToListAsync();

            foreach (var item in list)
                item.Response.ExtraData = item.ExtraData.ToDto();

            return list.Select(item => item.Response).ToList();
        }

        public static async Task<TestRunSessionResponse> MaterializeFirstOrDefaultAsync(this IQueryable<TestRunSession> sessions)
        {
            var item = await sessions.Project().FirstOrDefaultAsync();
            if (item == null)
                return null;
            item.Response.ExtraData = item.ExtraData.ToDto();
            return item.Response;
        }

        public static async Task<List<TestRunResponse>> MaterializeAllAsync(this IQueryable<TestRun> runs)
        {
            var list = await runs.Project().ToListAsync();

            foreach (var item in list)
            {
                item.Response.ExtraData = item.TestRunExtraData.ToDto();
                item.Response.TestCase.ExtraData = item.TestCaseExtraData.ToDto();
            }

            return list.Select(item => item.Response).ToList();
        }

        public static async Task<TestRunResponse> MaterializeFirstOrDefaultAsync(this IQueryable<TestRun> runs)
        {
            var item = await runs.Project().FirstOrDefaultAsync();
            if (item == null)
                return null;
            item.Response.ExtraData = item.TestRunExtraData.ToDto();
            item.Response.TestCase.ExtraData = item.TestCaseExtraData.ToDto();
            return item.Response;
        }
    }
}
