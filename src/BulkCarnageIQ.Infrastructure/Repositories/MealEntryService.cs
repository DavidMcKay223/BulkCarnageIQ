using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
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
                .Where(f => f.RecipeName.Equals(foodName))
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

        public async Task UpdateAsync(MealEntry entry)
        {
            var existingEntry = await _db.MealEntries.FindAsync(entry.Id);
            if (existingEntry != null)
            {
                //existingEntry.Date = entry.Date;
                //existingEntry.Day = entry.Day;
                //existingEntry.MealType = entry.MealType;
                //existingEntry.MealName = entry.MealName;
                //existingEntry.GroupName = entry.GroupName;
                existingEntry.PortionEaten = entry.PortionEaten;
                existingEntry.Calories = entry.Calories;
                existingEntry.Protein = entry.Protein;
                existingEntry.Carbs = entry.Carbs;
                existingEntry.Fats = entry.Fats;
                existingEntry.Fiber = entry.Fiber;
                existingEntry.MeasurementServings = entry.MeasurementServings;
                existingEntry.MeasurementType = entry.MeasurementType;
                
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _db.MealEntries.FindAsync(id);
            if (entry != null)
            {
                _db.MealEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 6)
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

        public async Task<MacroSummary> GetMacroSummaryAsync(DateOnly date, string userId)
        {
            var entries = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date == date)
                .ToListAsync();

            return new MacroSummary
            {
                Protein = entries.Sum(e => e.Protein),
                Carbs = entries.Sum(e => e.Carbs),
                Fats = entries.Sum(e => e.Fats),
                Fiber = entries.Sum(e => e.Fiber)
            };
        }

        public async Task<Dictionary<string, MacroSummary>> GetMacroSummariesByDateRangeAsync(DateOnly start, DateOnly end, string userId)
        {
            var entries = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date >= start && m.Date <= end)
                .ToListAsync();

            var grouped = entries
                .GroupBy(m => m.Day)
                .ToDictionary(
                    g => g.Key,
                    g => new MacroSummary
                    {
                        Protein = g.Sum(m => m.Protein),
                        Carbs = g.Sum(m => m.Carbs),
                        Fats = g.Sum(m => m.Fats),
                        Fiber = g.Sum(m => m.Fiber),
                    });

            // Define the full week in order
            var weekdayOrder = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // Return a sorted dictionary with all 7 days
            var result = weekdayOrder.ToDictionary(
                day => day,
                day => grouped.ContainsKey(day) ? grouped[day] : new MacroSummary()
            );

            return result;
        }
    }
}
