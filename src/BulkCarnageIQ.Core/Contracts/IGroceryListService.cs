using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IGroceryListService
    {
        Task<List<GroceryListItem>> GetListForUserAsync(string userId);
        Task<GroceryListItem> AddItemAsync(GroceryListItem item);
        Task RemoveItemAsync(int id, string userId);
        Task ToggleCompletedAsync(int id, string userId);
        Task ToggleFavoriteAsync(int id, string userId);
        Task ClearCompletedAsync(string userId);
        Task AutoGenerateFromRecentMealsAsync(string userId, int daysBack = 7);
    }
}
