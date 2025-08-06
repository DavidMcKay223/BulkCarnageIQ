using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IEngineService
    {
        Task<List<FoodItemSuggestion>> GetFoodItemSuggestionsAsync(List<MealEntry> mealEntries, UserProfile userProfile);
        Task<List<FoodItemSuggestionGroup>> GetFoodItemGroupSuggestionsAsync(List<MealEntry> mealEntries, UserProfile userProfile);
    }
}
