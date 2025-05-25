using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task SeedFoodItemsAsync(this AppDbContext context, IEnumerable<FoodItem> foodItems)
        {
            await context.FoodItems.AddRangeAsync(foodItems);
            await context.SaveChangesAsync();
        }

        public static async Task SeedGroceryListItemsAsync(this AppDbContext context, IEnumerable<GroceryListItem> items)
        {
            await context.GroceryListItems.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }

        public static async Task SeedMealEntriesAsync(this AppDbContext context, IEnumerable<MealEntry> items)
        {
            await context.MealEntries.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }
    }
}
