using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
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

        Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 6);
        Task<MacroSummary> GetMacroSummaryAsync(DateOnly date, string userId);
        Task<Dictionary<string, MacroSummary>> GetMacroSummariesByDateRangeAsync(DateOnly start, DateOnly end, string userId);
    }
}
