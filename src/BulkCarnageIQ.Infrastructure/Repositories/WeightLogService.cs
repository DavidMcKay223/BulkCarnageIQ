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

        public async Task<List<WeightLog>> GetUserLogsAsync(string userId, bool showProjectionWeight)
        {
            var results = await _db.WeightLogs
                .Where(w => w.UserId == userId)
                .OrderBy(w => w.Date)
                .ToListAsync();

            if (showProjectionWeight && results.Any())
            {
                // Get user's meals (assuming MealEntries are in the same DbContext)
                var last7Days = DateOnly.FromDateTime(DateTime.Today.AddDays(-6));
                var recentMeals = await _db.MealEntries
                    .Where(m => m.UserId == userId && m.Date >= last7Days)
                    .ToListAsync();

                if (recentMeals.Any())
                {
                    float totalCalories = recentMeals.Sum(m => m.Calories);
                    int daysTracked = (int)(DateTime.Today - recentMeals.Min(m => m.Date).ToDateTime(TimeOnly.MinValue)).TotalDays + 1;
                    float avgDailyCalories = totalCalories / daysTracked;

                    // Constants
                    const float EstimatedMaintenanceCalories = 2800f;
                    const float CaloriesPerPound = 3500f;

                    float dailyCalorieDelta = avgDailyCalories - EstimatedMaintenanceCalories;
                    float dailyWeightChange = dailyCalorieDelta / CaloriesPerPound;

                    var currentWeight = results.Last().WeightLbs;

                    // Project weights
                    int numberOfWeeksToProject = 10;
                    var random = new Random();

                    for (int week = 1; week <= numberOfWeeksToProject; week++)
                    {
                        int daysToAdd = week * 7;
                        float projectedWeight = currentWeight + (dailyWeightChange * daysToAdd);

                        // Add random variation ±1.0 lbs
                        float randomWiggle = (float)(random.NextDouble() * 2 - 1);  // random between -1 and +1
                        projectedWeight += randomWiggle;

                        // Optional: Clamp projected weight to not be negative
                        projectedWeight = Math.Max(projectedWeight, 50);  // nobody weighs less than 50 lbs

                        results.Add(new WeightLog
                        {
                            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(daysToAdd)),
                            WeightLbs = projectedWeight,
                            UserId = userId
                        });
                    }
                }
            }

            return results;
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
