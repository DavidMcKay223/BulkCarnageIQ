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
    public class MealEntryService : IMealEntryService
    {
        private readonly AppDbContext _db;

        public MealEntryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<FoodItem?> GetFoodItemByNameAsync(string foodName)
        {
            return await _db.FoodItems
                .Where(f => f.RecipeName.Contains(foodName))
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> SearchFoodNamesAsync(string query)
        {
            return await _db.FoodItems
                .Where(f => f.RecipeName.Contains(query))
                .Select(f => f.RecipeName)
                .Distinct()
                .OrderBy(name => name)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<MealEntry>> GetAllAsync(string userId)
        {
            // Define the desired order of meal types
            List<string> mealTypeOrder = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack" };

            // Create a dictionary for quick lookup of the order index
            Dictionary<string, int> mealTypeOrderDict = mealTypeOrder
                .Select((mealType, index) => new { mealType, index })
                .ToDictionary(x => x.mealType, x => x.index);

            var entries = await _db.MealEntries
                .Where(e => e.UserId == userId)
                .ToListAsync(); // fetch all relevant entries first

            // Do the custom ordering in memory
            return entries
                .OrderByDescending(e => e.Date)
                .ThenBy(e => mealTypeOrderDict.TryGetValue(e.MealType, out var index) ? index : 4)
                .ToList();
        }

        public async Task AddAsync(MealEntry entry)
        {
            _db.MealEntries.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 7)
        {
            var since = DateOnly.FromDateTime(DateTime.Today.AddDays(-daysBack));

            var entries = await _db.MealEntries
                .Where(me => me.UserId == userId && me.Date >= since)
                .ToListAsync();

            return entries
                .GroupBy(me => me.Day)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(me => me.Calories)
                );
        }
    }
}
