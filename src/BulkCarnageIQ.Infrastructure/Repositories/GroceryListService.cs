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
    public class GroceryListService : IGroceryListService
    {
        private readonly AppDbContext _db;

        public GroceryListService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroceryListItem>> GetListForUserAsync(string userId)
        {
            return await _db.GroceryListItems
                .Where(item => item.UserId == userId)
                .OrderByDescending(i => i.IsFavorite)
                .ThenBy(i => i.IsCompleted)
                .ThenBy(i => i.RecipeName)
                .ToListAsync();
        }

        public async Task<GroceryListItem> AddItemAsync(GroceryListItem item)
        {
            _db.GroceryListItems.Add(item);
            await _db.SaveChangesAsync();

            return item;
        }

        public async Task RemoveItemAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                _db.GroceryListItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ToggleCompletedAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                item.IsCompleted = !item.IsCompleted;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ToggleFavoriteAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                item.IsFavorite = !item.IsFavorite;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCompletedAsync(string userId)
        {
            var completedItems = await _db.GroceryListItems
                .Where(x => x.UserId == userId && x.IsCompleted)
                .ToListAsync();

            if (completedItems.Any())
            {
                _db.GroceryListItems.RemoveRange(completedItems);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AutoGenerateFromRecentMealsAsync(string userId, int daysBack = 7)
        {
            var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-daysBack));

            var recentMeals = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date >= cutoff)
                .GroupBy(m => m.MealName)
                .Select(g => g.First())
                .ToListAsync();

            foreach (var meal in recentMeals)
            {
                var exists = await _db.GroceryListItems.AnyAsync(x =>
                    x.RecipeName == meal.MealName && x.UserId == userId);

                if (!exists)
                {
                    var tempItem = await _db.FoodItems
                        .FirstOrDefaultAsync(x => x.RecipeName == meal.MealName);

                    _db.GroceryListItems.Add(new GroceryListItem
                    {
                        RecipeName = meal.MealName,
                        GroupName = tempItem?.GroupName,
                        UserId = userId,
                        CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        MeasurementType = meal.MeasurementType,
                        MeasurementServings = meal.MeasurementServings,
                        IsFavorite = false,
                        IsCompleted = false
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
