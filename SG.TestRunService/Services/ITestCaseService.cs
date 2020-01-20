using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestCaseService
    {
        Task<TestCaseResponse> InsertAsync(TestCaseRequest testCase);
        Task<IReadOnlyList<TestCaseResponse>> InsertAsync(IEnumerable<TestCaseRequest> testCases);
        Task<IReadOnlyList<TestCaseResponse>> GetAllAsync(string teamProject);
        Task<IReadOnlyList<TestCaseResponse>> GetAllAsync(string teamProject, IEnumerable<string> fieldNames);
        Task<TestCaseResponse> GetAsync(int id);
        Task<IReadOnlyList<int>> GetAzureTestCaseIdsAsync(string teamProject);
    }
}
