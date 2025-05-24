# Directory: Repositories

## File: FoodItemService.cs

```C#
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

        public async Task<FoodItem?> GetFoodItemByName(string recipeName)
        {
            var foodItem = await _db.FoodItems
                .FirstOrDefaultAsync(f => f.RecipeName == recipeName);

            return foodItem;
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

```

## File: GroceryListService.cs

```C#
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
    public class GroceryListService : IGroceryListService
    {
        private readonly AppDbContext _db;

        public GroceryListService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroceryListItem>> GetListForUserAsync(string userId)
        {
            return await _db.GroceryListItems
                .Where(item => item.UserId == userId)
                .OrderByDescending(i => i.IsFavorite)
                .ThenBy(i => i.IsCompleted)
                .ThenBy(i => i.RecipeName)
                .ToListAsync();
        }

        public async Task<GroceryListItem> AddItemAsync(GroceryListItem item)
        {
            _db.GroceryListItems.Add(item);
            await _db.SaveChangesAsync();

            return item;
        }

        public async Task RemoveItemAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                _db.GroceryListItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ToggleCompletedAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                item.IsCompleted = !item.IsCompleted;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ToggleFavoriteAsync(int id, string userId)
        {
            var item = await _db.GroceryListItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is not null)
            {
                item.IsFavorite = !item.IsFavorite;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCompletedAsync(string userId)
        {
            var completedItems = await _db.GroceryListItems
                .Where(x => x.UserId == userId && x.IsCompleted)
                .ToListAsync();

            if (completedItems.Any())
            {
                _db.GroceryListItems.RemoveRange(completedItems);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AutoGenerateFromRecentMealsAsync(string userId, int daysBack = 7)
        {
            var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-daysBack));

            var recentMeals = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date >= cutoff)
                .GroupBy(m => m.MealName)
                .Select(g => g.First())
                .ToListAsync();

            foreach (var meal in recentMeals)
            {
                var exists = await _db.GroceryListItems.AnyAsync(x =>
                    x.RecipeName == meal.MealName && x.UserId == userId);

                if (!exists)
                {
                    var tempItem = await _db.FoodItems
                        .FirstOrDefaultAsync(x => x.RecipeName == meal.MealName);

                    _db.GroceryListItems.Add(new GroceryListItem
                    {
                        RecipeName = meal.MealName,
                        GroupName = tempItem?.GroupName,
                        UserId = userId,
                        CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        MeasurementType = meal.MeasurementType,
                        MeasurementServings = meal.MeasurementServings,
                        IsFavorite = false,
                        IsCompleted = false
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}

```

## File: GroupFoodService.cs

```C#
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
    public class GroupFoodService : IGroupFoodService
    {
        private readonly AppDbContext _db;

        public GroupFoodService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroupFoodItem>> GetAllAsync()
        {
            return await _db.GroupFoodItems
                .Include(g => g.Entries)
                .ToListAsync();
        }

        public async Task<List<FoodItem>> GetFoodItemByGroupName(string groupName)
        {
            var group = await _db.GroupFoodItems
                .Include(g => g.Entries)
                .FirstOrDefaultAsync(g => g.GroupName == groupName);

            if (group == null || group.Entries.Count == 0)
            {
                return new List<FoodItem>();
            }

            var recipeNames = group.Entries.Select(e => e.RecipeName).ToList();

            return await _db.FoodItems
                .Where(f => f.RecipeName.Equals(recipeNames))
                .ToListAsync();
        }

        public async Task<List<string>> GetGroupNames()
        {
            return await _db.GroupFoodItems
                .Select(f => f.GroupName)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
        }
    }
}

```

## File: MealEntryService.cs

```C#
using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Carnage.Report;
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
    public class MealEntryService : IMealEntryService
    {
        private readonly AppDbContext _db;

        public MealEntryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<FoodItem?> GetFoodItemByNameAsync(string foodName)
        {
            return await _db.FoodItems
                .Where(f => f.RecipeName.Equals(foodName))
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> SearchFoodNamesAsync(string query)
        {
            return await _db.FoodItems
                .Where(f => f.RecipeName.Contains(query))
                .Select(f => f.RecipeName)
                .Distinct()
                .OrderBy(name => name)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<MealEntry>> GetAllAsync(string userId)
        {
            // Define the desired order of meal types
            List<string> mealTypeOrder = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack" };

            // Create a dictionary for quick lookup of the order index
            Dictionary<string, int> mealTypeOrderDict = mealTypeOrder
                .Select((mealType, index) => new { mealType, index })
                .ToDictionary(x => x.mealType, x => x.index);

            var entries = await _db.MealEntries
                .Where(e => e.UserId == userId)
                .ToListAsync(); // fetch all relevant entries first

            // Do the custom ordering in memory
            return entries
                .OrderByDescending(e => e.Date)
                .ThenBy(e => mealTypeOrderDict.TryGetValue(e.MealType, out var index) ? index : 4)
                .ToList();
        }

        public async Task AddAsync(MealEntry entry)
        {
            _db.MealEntries.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(MealEntry entry)
        {
            var existingEntry = await _db.MealEntries.FindAsync(entry.Id);
            if (existingEntry != null)
            {
                //existingEntry.Date = entry.Date;
                //existingEntry.Day = entry.Day;
                //existingEntry.MealType = entry.MealType;
                //existingEntry.MealName = entry.MealName;
                //existingEntry.GroupName = entry.GroupName;
                existingEntry.PortionEaten = entry.PortionEaten;
                existingEntry.Calories = entry.Calories;
                existingEntry.Protein = entry.Protein;
                existingEntry.Carbs = entry.Carbs;
                existingEntry.Fats = entry.Fats;
                existingEntry.Fiber = entry.Fiber;
                existingEntry.MeasurementServings = entry.MeasurementServings;
                existingEntry.MeasurementType = entry.MeasurementType;
                
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _db.MealEntries.FindAsync(id);
            if (entry != null)
            {
                _db.MealEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, float>> GetCaloriesByDayAsync(string userId, int daysBack = 6)
        {
            var since = DateOnly.FromDateTime(DateTime.Today.AddDays(-daysBack));

            var entries = await _db.MealEntries
                .Where(me => me.UserId == userId && me.Date >= since)
                .ToListAsync();

            return entries
                .GroupBy(me => me.Day)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(me => me.Calories)
                );
        }

        public async Task<MacroSummary> GetMacroSummaryAsync(DateOnly date, string userId)
        {
            var entries = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date == date)
                .ToListAsync();

            return new MacroSummary
            {
                Protein = entries.Sum(e => e.Protein),
                Carbs = entries.Sum(e => e.Carbs),
                Fats = entries.Sum(e => e.Fats),
                Fiber = entries.Sum(e => e.Fiber)
            };
        }

        public async Task<Dictionary<string, MacroSummary>> GetMacroSummariesByDateRangeAsync(DateOnly start, DateOnly end, string userId)
        {
            var entries = await _db.MealEntries
                .Where(m => m.UserId == userId && m.Date >= start && m.Date <= end)
                .ToListAsync();

            var grouped = entries
                .GroupBy(m => m.Day)
                .ToDictionary(
                    g => g.Key,
                    g => new MacroSummary
                    {
                        Protein = g.Sum(m => m.Protein),
                        Carbs = g.Sum(m => m.Carbs),
                        Fats = g.Sum(m => m.Fats),
                        Fiber = g.Sum(m => m.Fiber),
                    });

            // Define the full week in order
            var weekdayOrder = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // Return a sorted dictionary with all 7 days
            var result = weekdayOrder.ToDictionary(
                day => day,
                day => grouped.ContainsKey(day) ? grouped[day] : new MacroSummary()
            );

            return result;
        }
    }
}

```

## File: UserProfileService.cs

```C#
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
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _db;

        public UserProfileService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfile> GetUserProfile(string userName)
        {
            var profile = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.UserName == userName);

            return profile ?? new UserProfile
            {
                UserName = userName,
                ActivityLevel = "sedentary",
                GoalType = "maintain",
                Age = 25f,
                ProteinGoal = 120f,
                CarbsGoal = 220f,
                FatGoal = 80f,
                FiberGoal = 25f,
                CalorieGoal = 2800f,
            };
        }

        public async Task SaveUserProfile(UserProfile userProfile)
        {
            var existing = await _db.UserProfiles
                .FindAsync(userProfile.UserName);

            if (existing == null)
            {
                await _db.UserProfiles.AddAsync(userProfile);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(userProfile);
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateUserGoals(string userName)
        {
            var userProfile = await GetUserProfile(userName);
            userProfile.CalculateGoals();
            await SaveUserProfile(userProfile);
        }
    }
}

```

## File: WeightLogService.cs

```C#
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
    public class WeightLogService : IWeightLogService
    {
        private readonly AppDbContext _db;

        public WeightLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<WeightLog>> GetUserLogsAsync(string userId, bool showProjectionWeight)
        {
            var results = await _db.WeightLogs
                .Where(w => w.UserId == userId)
                .OrderBy(w => w.Date)
                .ToListAsync();

            if (showProjectionWeight && results.Any())
            {
                // Get user's meals (assuming MealEntries are in the same DbContext)
                var last7Days = DateOnly.FromDateTime(DateTime.Today.AddDays(-6));
                var recentMeals = await _db.MealEntries
                    .Where(m => m.UserId == userId && m.Date >= last7Days)
                    .ToListAsync();

                if (recentMeals.Any())
                {
                    float totalCalories = recentMeals.Sum(m => m.Calories);
                    int daysTracked = (int)(DateTime.Today - recentMeals.Min(m => m.Date).ToDateTime(TimeOnly.MinValue)).TotalDays + 1;
                    float avgDailyCalories = totalCalories / daysTracked;

                    // Constants
                    const float EstimatedMaintenanceCalories = 2800f;
                    const float CaloriesPerPound = 3500f;

                    float dailyCalorieDelta = avgDailyCalories - EstimatedMaintenanceCalories;
                    float dailyWeightChange = dailyCalorieDelta / CaloriesPerPound;

                    var currentWeight = results.Last().WeightLbs;

                    // Project weights
                    int numberOfWeeksToProject = 2;
                    var random = new Random();

                    for (int week = 1; week <= numberOfWeeksToProject; week++)
                    {
                        int daysToAdd = week * 7;
                        float projectedWeight = currentWeight + (dailyWeightChange * daysToAdd);

                        // Add random variation ±1.0 lbs
                        float randomWiggle = (float)(random.NextDouble() * 2 - 1);  // random between -1 and +1
                        projectedWeight += randomWiggle;

                        // Optional: Clamp projected weight to not be negative
                        projectedWeight = Math.Max(projectedWeight, 50);  // nobody weighs less than 50 lbs

                        results.Add(new WeightLog
                        {
                            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(daysToAdd)),
                            WeightLbs = projectedWeight,
                            UserId = userId
                        });
                    }
                }
            }

            return results;
        }

        public async Task AddOrUpdateLogAsync(string userId, DateOnly date, float weightLbs)
        {
            if(string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var log = await _db.WeightLogs.FirstOrDefaultAsync(w => w.UserId == userId && w.Date == date);
            if (log is null)
            {
                _db.WeightLogs.Add(new WeightLog { UserId = userId, Date = date, WeightLbs = weightLbs });
            }
            else
            {
                log.WeightLbs = weightLbs;
            }
            await _db.SaveChangesAsync();
        }
    }
}

```

