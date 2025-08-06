using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Engine;
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
    public class EngineService : IEngineService
    {
        private readonly AppDbContext _db;

        public EngineService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<FoodItemSuggestionGroup>> GetFoodItemGroupSuggestionsAsync(List<MealEntry> mealEntries, UserProfile userProfile)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FoodItemSuggestion>> GetFoodItemSuggestionsAsync(List<MealEntry> mealEntries, UserProfile userProfile)
        {
            float totalProtein = mealEntries.Sum(m => m.Protein);
            float totalCarbs = mealEntries.Sum(m => m.Carbs);
            float totalFats = mealEntries.Sum(m => m.Fats);
            float totalFiber = mealEntries.Sum(m => m.Fiber);
            float totalCalories = mealEntries.Sum(m => m.Calories);

            float remainingProtein = Math.Max(userProfile.ProteinGoal - totalProtein, 0f);
            float remainingCarbs = Math.Max(userProfile.CarbsGoal - totalCarbs, 0f);
            float remainingFats = Math.Max(userProfile.FatGoal - totalFats, 0f);
            float remainingFiber = Math.Max(userProfile.FiberGoal - totalFiber, 0f);
            float remainingCalories = Math.Max(userProfile.CalorieGoal - totalCalories, 0f);

            var items = await _db.FoodItems.ToListAsync();

            List<FoodItemSuggestion> suggestions = items.Select(f => new FoodItemSuggestion
            {
                RecipeName = f.RecipeName,
                BrandType = f.BrandType,
                GroupName = f.GroupName,
                Servings = f.Servings,
                CaloriesPerServing = f.CaloriesPerServing,
                MeasurementServings = f.MeasurementServings,
                MeasurementType = f.MeasurementType,
                Protein = f.Protein,
                Carbs = f.Carbs,
                Fats = f.Fats,
                Fiber = f.Fiber,
                IsBreakfast = f.IsBreakfast,
                IsLunch = f.IsLunch,
                IsDinner = f.IsDinner,
                IsSnack = f.IsSnack,
                Link = f.Link,
                PictureLink = f.PictureLink
            })
            .Where(f =>
                f.TotalCalories <= remainingCalories &&
                (f.Protein <= remainingProtein || remainingProtein <= 0f) &&
                (f.Carbs <= remainingCarbs || remainingCarbs <= 0f) &&
                (f.Fats <= remainingFats || remainingFats <= 0f))
            .OrderByDescending(f => f.Protein)
            .ThenByDescending(f => f.Fiber)
            .ThenByDescending(f => f.TotalCalories)
            .ThenByDescending(f => f.Fats)
            .ThenByDescending(f => f.Carbs)
            .ToList();

            return suggestions;
        }
    }
}
