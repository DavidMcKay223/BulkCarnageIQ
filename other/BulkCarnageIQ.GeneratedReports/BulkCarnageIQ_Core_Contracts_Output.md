# Directory: Contracts

## File: IFoodItemService.cs

```C#
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IFoodItemService
    {
        Task<FoodItem?> GetFoodItemByName(string recipeName);
        Task<List<FoodItem>> GetAllAsync();
        Task<Dictionary<string, FoodItem>> GetAllDictionaryAsync(List<string> recipeName);
    }
}

```

## File: IGroceryListService.cs

```C#
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

```

## File: IGroupFoodService.cs

```C#
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IGroupFoodService
    {
        Task<List<GroupFoodItem>> GetAllAsync();
        Task<List<FoodItem>> GetFoodItemByGroupName(string groupName);
        Task<List<string>> GetGroupNames();
    }
}

```

## File: IMealEntryService.cs

```C#
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
        Task UpdateAsync(MealEntry entry);
        Task DeleteAsync(int Id);

        Task<FoodItem?> GetFoodItemByNameAsync(string foodName);
        Task<List<string>> SearchFoodNamesAsync(string query);

        Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 6);
        Task<MacroSummary> GetMacroSummaryAsync(DateOnly date, string userId);
        Task<Dictionary<string, MacroSummary>> GetMacroSummariesByDateRangeAsync(DateOnly start, DateOnly end, string userId);
    }
}

```

## File: IUserProfileService.cs

```C#
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfile(string userName);
        Task SaveUserProfile(UserProfile userProfile);
        Task UpdateUserGoals(string userName);
    }
}

```

## File: IWeightLogService.cs

```C#
using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IWeightLogService
    {
        Task<List<WeightLog>> GetUserLogsAsync(string userId, bool showProjectionWeight);
        Task AddOrUpdateLogAsync(string userId, DateOnly date, float weightLbs);
    }
}

```

