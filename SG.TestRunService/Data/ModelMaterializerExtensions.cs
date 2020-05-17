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
            item.Response.ExtraData = item.ExtraData.ToDto();
            return item.Response;
        }
    }
}
