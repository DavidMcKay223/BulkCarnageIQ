using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Infrastructure.Repositories
{
    public class WeightLogService : IWeightLogService
    {
        private readonly AppDbContext _db;

        public WeightLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<WeightLog>> GetUserLogsAsync(string userId)
        {
            return await _db.WeightLogs.Where(w => w.UserId == userId)
                            .OrderBy(w => w.Date)
                            .ToListAsync();
        }

        public async Task AddOrUpdateLogAsync(string userId, DateOnly date, float weightLbs)
        {
            if(string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var log = await _db.WeightLogs.FirstOrDefaultAsync(w => w.UserId == userId && w.Date == date);
            if (log is null)
            {
                _db.WeightLogs.Add(new WeightLog { UserId = userId, Date = date, WeightLbs = weightLbs });
            }
            else
            {
                log.WeightLbs = weightLbs;
            }
            await _db.SaveChangesAsync();
        }
    }
}
