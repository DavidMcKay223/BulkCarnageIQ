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
    public class FoodItemService : IFoodItemService
    {
        private readonly AppDbContext _db;

        public FoodItemService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<FoodItem>> GetAllAsync()
        {
            return await _db.FoodItems
                .ToListAsync();
        }

        public async Task<Dictionary<string, FoodItem>> GetAllDictionaryAsync(List<string> recipeNames)
        {
            if (recipeNames == null || !recipeNames.Any())
            {
                return new Dictionary<string, FoodItem>();
            }

            var foodItems = await _db.FoodItems
                .Where(f => recipeNames.Contains(f.RecipeName))
                .ToListAsync();

            var foodItemsDictionary = foodItems.ToDictionary(f => f.RecipeName, f => f);

            return foodItemsDictionary;
        }
    }
}
