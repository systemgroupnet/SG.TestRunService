using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IBaseDbService _dbService;

        public TestCaseService(IBaseDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<TestCaseResponse> InsertAsync(TestCaseRequest testCaseRequest)
        {
            var testCase = testCaseRequest.ToDataModel();
            await _dbService.InsertAsync(testCase);
            return testCase.ToResponse();
        }

        public async Task<IReadOnlyList<TestCaseResponse>> InsertAsync(IEnumerable<TestCaseRequest> testCaseRequests)
        {
            var testCases = testCaseRequests.Select(ModelMappingExtensions.ToDataModel).ToList();
            await _dbService.InsertAsync(testCases);
            return testCases.ConvertAll(ModelMappingExtensions.ToResponse);
        }

        public Task<TestCaseResponse> GetAsync(int id)
        {
            return _dbService.Query<TestCase>(id).Project().FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TestCaseResponse>> GetAllAsync(string teamProject)
        {
            return await _dbService
                .Query<TestCase>(tc => tc.TeamProject == teamProject)
                .Project()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<int>> GetAzureTestCaseIdsAsync(string teamProject)
        {
            return await _dbService
                .Query<TestCase>()
                .Where(tc => tc.TeamProject == teamProject)
                .Select(tc => tc.AzureTestCaseId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TestCaseResponse>> GetAllAsync(string teamProject, IEnumerable<string> fieldNames)
        {
            var names = fieldNames.ToList();
            if (names.Count == 2 &&
                names.Contains(nameof(TestCaseResponse.Id)) &&
                names.Contains(nameof(TestCaseResponse.AzureTestCaseId)))
            {
                return await _dbService.Query<TestCase>()
                        .Where(tc => tc.TeamProject == teamProject)
                        .Select(tc => new TestCaseResponse()
                        {
                            Id = tc.Id,
                            AzureTestCaseId = tc.AzureTestCaseId
                        })
                        .ToListAsync();
            }
            throw new NotSupportedException("Currently only selecting `Id` and `AzureTestCaseId` is supported!");
        }

        public async Task<TestCaseResponse> DeleteAsync(int testCaseId)
        {
            var testCase = await _dbService.DeleteAsync<TestCase>(testCaseId);
            if (testCase == null)
                return null;
            return testCase.ToResponse();
        }
    }
}
