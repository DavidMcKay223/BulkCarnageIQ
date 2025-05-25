using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Tests.SharedUtils.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Tests.SharedUtils.Seed
{
    public static class SampleDataSeeder
    {
        public static List<FoodItem> GenerateFoodItems(int count = 10)
        {
            var faker = FoodItemGenerator.GetFaker();
            var foodItems = faker.Generate(count);
            return foodItems;
        }

        public static List<MealEntry> GenerateMealEntries(string userId, int count = 10)
        {
            var faker = MealEntryGenerator.GetFaker(userId);
            return faker.Generate(count);
        }

        public static List<GroceryListItem> GenerateGroceryListItems(string userId, int count = 10)
        {
            var faker = GroceryListItemGenerator.GetFaker(userId);
            return faker.Generate(count);
        }
    }
}
