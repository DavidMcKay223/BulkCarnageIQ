using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IWeightLogService
    {
        Task<List<WeightLog>> GetUserLogsAsync(string userId);
        Task AddOrUpdateLogAsync(string userId, DateOnly date, float weightLbs);
    }
}
