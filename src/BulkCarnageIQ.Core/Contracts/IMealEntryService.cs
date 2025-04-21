using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IMealEntryService
    {
        Task<List<MealEntry>> GetAllAsync(string userID);
        Task AddAsync(MealEntry entry);

        Task<FoodItem?> GetFoodItemByNameAsync(string foodName);
        Task<List<string>> SearchFoodNamesAsync(string query);

        Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 7);
    }
}
