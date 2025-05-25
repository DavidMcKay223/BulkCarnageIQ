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
    }
}
