using BulkCarnageIQ.Core.Carnage.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Report
{
    public class FoodItemFilter
    {
        // Flags for filtering
        public bool IsHighProtein { get; set; }
        public bool IsLowCarb { get; set; }
        public bool IsKeto { get; set; }
        public bool IsBulkMeal { get; set; }
        public bool IsHighCarb { get; set; }
        public bool IsHighFiber { get; set; }
        public bool IsHighFat { get; set; }
        public bool IsBalancedMeal { get; set; }
        public bool IsLowProtein { get; set; }

        // Optional properties for grouping/filtering
        public string? SelectedRecipeName { get; set; }
        public string? SelectedBrandType { get; set; }
        public string? SelectedGroupName { get; set; }

        // Constructor to initialize with default values
        public FoodItemFilter()
        {
            IsHighProtein = false;
            IsLowCarb = false;
            IsKeto = false;
            IsBulkMeal = false;
            IsHighCarb = false;
            IsHighFiber = false;
            IsHighFat = false;
            IsBalancedMeal = false;
            IsLowProtein = false;
            SelectedRecipeName = null;
            SelectedBrandType = null;
            SelectedGroupName = null;
        }

        public static readonly Dictionary<string, int> FoodGroupOrder = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Meat & Seafood"] = 1,
            ["Nutrition"] = 2,
            ["Dairy & Eggs"] = 3,
            ["Fruits & Vegetables"] = 4,
            ["Breads & Grains"] = 5,
            ["Yogurt"] = 6,
            ["Soup"] = 7,
            ["Prepared Meals"] = 8,
            ["Frozen Meals"] = 8,
            ["Homemade"] = 8,
            ["Canned"] = 9,
            ["Bakery"] = 10,
            ["Appetizers"] = 10,
            ["Restaurant"] = 11,
            ["Fast Food"] = 11,
            ["Movie Theater"] = 11,
            ["Pizza"] = 12,
            ["Sandwich"] = 12,
            ["Salad"] = 12,
            ["Burrito"] = 12,
            ["Snacks"] = 13,
            ["Condiments & Sauces"] = 14,
            ["Dessert"] = 15,
            ["Candy"] = 15,
            ["Beverages"] = 15
        };

        // Method to apply filters to a collection of FoodItem objects
        public IEnumerable<FoodItem> ApplyFilters(IEnumerable<FoodItem> foodItems)
        {
            return foodItems.Where(item =>
                (string.IsNullOrEmpty(SelectedRecipeName) || item.RecipeName.Contains(SelectedRecipeName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(SelectedBrandType) || item.BrandType.Contains(SelectedBrandType, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(SelectedGroupName) || (item.GroupName != null && item.GroupName.Contains(SelectedGroupName, StringComparison.OrdinalIgnoreCase))) &&
                (!IsHighProtein || item.IsHighProtein) &&
                (!IsLowCarb || item.IsLowCarb) &&
                (!IsKeto || item.IsKeto) &&
                (!IsBulkMeal || item.IsBulkMeal) &&
                (!IsHighCarb || item.IsHighCarb) &&
                (!IsHighFiber || !item.IsLowFiber) &&
                (!IsHighFat || item.IsHighFat) &&
                (!IsBalancedMeal || item.IsBalancedMeal) &&
                (!IsLowProtein || item.IsLowProtein)
            )
            .OrderByDescending(f => f.Protein)
            .ThenByDescending(f => f.Fiber)
            .ThenByDescending(f => f.TotalCalories)
            .ThenByDescending(f => f.Fats)
            .ThenByDescending(f => f.Carbs);
        }

        public IEnumerable<FoodItem> ApplyOrder(IEnumerable<FoodItem> foodItems, UserProfile _currentUserProfile, MacroSummary _macroSummary)
        {
            return foodItems
                .OrderBy(f => FoodGroupOrder.ContainsKey(f.GroupName!) ? FoodGroupOrder[f.GroupName!] : int.MaxValue)
                .ThenByDescending(f =>
                {
                    // We don't want to see zero-macro foods first.
                    // If a food has no protein, fiber, or calories, it gets a minimum score.
                    if (f.Protein <= 0 && f.Fiber <= 0 && f.TotalCalories <= 0)
                    {
                        return float.MinValue;
                    }

                    // Current macro status
                    float remainingProtein = _currentUserProfile.ProteinGoal - _macroSummary.Protein;
                    float remainingFiber = _currentUserProfile.FiberGoal - _macroSummary.Fiber;
                    float remainingCalories = _currentUserProfile.CalorieGoal - _macroSummary.Calories;
                    float remainingFat = _currentUserProfile.FatGoal - _macroSummary.Fats;
                    float remainingCarbs = _currentUserProfile.CarbsGoal - _macroSummary.Carbs;

                    float score = 0;

                    // Use multipliers to define priority
                    const float proteinMultiplier = 1000000000;
                    const float fiberMultiplier = 1000000000; // Giving fiber top priority in this scenario
                    const float calorieMultiplier = 10000;
                    const float fatMultiplier = 1000;
                    const float carbMultiplier = 100;

                    // 1. Fiber Priority: This is the only macro you're under on, so it gets top priority.
                    if (remainingFiber > 0)
                    {
                        score += Math.Min(f.Fiber, remainingFiber) * fiberMultiplier;
                    }
                    else
                    {
                        // Even if over, we assume fiber is beneficial, so we still reward it.
                        score += f.Fiber * fiberMultiplier;
                    }

                    // 2. Calorie Penalty: You are over on calories, so we heavily penalize high-calorie foods.
                    if (remainingCalories <= 0)
                    {
                        score -= f.TotalCalories * calorieMultiplier;
                    }
                    else
                    {
                        // If you were under, this would be the logic to reward low-calorie foods
                        score -= f.TotalCalories * calorieMultiplier;
                    }

                    // 3. Protein Penalty: You are over on protein.
                    if (remainingProtein <= 0)
                    {
                        score -= f.Protein * proteinMultiplier;
                    }
                    else
                    {
                        score += Math.Min(f.Protein, remainingProtein) * proteinMultiplier;
                    }

                    // 4. Fat Penalty: You are over on fat.
                    if (remainingFat <= 0)
                    {
                        score -= f.Fats * fatMultiplier;
                    }
                    else
                    {
                        score -= f.Fats * fatMultiplier;
                    }

                    // 5. Carb Penalty: You are over on carbs.
                    if (remainingCarbs <= 0)
                    {
                        score -= f.Carbs * carbMultiplier;
                    }
                    else
                    {
                        score -= f.Carbs * carbMultiplier;
                    }

                    return score;
                });
        }

        public IEnumerable<FoodItemSuggestion> ApplyFilters(IEnumerable<FoodItemSuggestion> foodItems)
        {
            return foodItems.Where(item =>
                (string.IsNullOrEmpty(SelectedRecipeName) || item.RecipeName.Contains(SelectedRecipeName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(SelectedBrandType) || item.BrandType.Contains(SelectedBrandType, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(SelectedGroupName) || (item.GroupName != null && item.GroupName.Contains(SelectedGroupName, StringComparison.OrdinalIgnoreCase))) &&
                (!IsHighProtein || item.IsHighProtein) &&
                (!IsLowCarb || item.IsLowCarb) &&
                (!IsKeto || item.IsKeto) &&
                (!IsBulkMeal || item.IsBulkMeal) &&
                (!IsHighCarb || item.IsHighCarb) &&
                (!IsHighFiber || !item.IsLowFiber) &&
                (!IsHighFat || item.IsHighFat) &&
                (!IsBalancedMeal || item.IsBalancedMeal) &&
                (!IsLowProtein || item.IsLowProtein)
            )
            .OrderByDescending(f => f.Protein)
            .ThenByDescending(f => f.Fiber)
            .ThenByDescending(f => f.TotalCalories)
            .ThenByDescending(f => f.Fats)
            .ThenByDescending(f => f.Carbs);
        }

        public IEnumerable<FoodItemSuggestion> ApplyOrder(IEnumerable<FoodItemSuggestion> foodItems, UserProfile _currentUserProfile, MacroSummary _macroSummary)
        {
            return foodItems
                .OrderBy(f => FoodGroupOrder.ContainsKey(f.GroupName!) ? FoodGroupOrder[f.GroupName!] : int.MaxValue)
                .ThenByDescending(f =>
                {
                    // We don't want to see zero-macro foods first.
                    // If a food has no protein, fiber, or calories, it gets a minimum score.
                    if (f.Protein <= 0 && f.Fiber <= 0 && f.TotalCalories <= 0)
                    {
                        return float.MinValue;
                    }

                    // Current macro status
                    float remainingProtein = _currentUserProfile.ProteinGoal - _macroSummary.Protein;
                    float remainingFiber = _currentUserProfile.FiberGoal - _macroSummary.Fiber;
                    float remainingCalories = _currentUserProfile.CalorieGoal - _macroSummary.Calories;
                    float remainingFat = _currentUserProfile.FatGoal - _macroSummary.Fats;
                    float remainingCarbs = _currentUserProfile.CarbsGoal - _macroSummary.Carbs;

                    float score = 0;

                    // Use multipliers to define priority
                    const float proteinMultiplier = 1000000000;
                    const float fiberMultiplier = 1000000000; // Giving fiber top priority in this scenario
                    const float calorieMultiplier = 10000;
                    const float fatMultiplier = 1000;
                    const float carbMultiplier = 100;

                    // 1. Fiber Priority: This is the only macro you're under on, so it gets top priority.
                    if (remainingFiber > 0)
                    {
                        score += Math.Min(f.Fiber, remainingFiber) * fiberMultiplier;
                    }
                    else
                    {
                        // Even if over, we assume fiber is beneficial, so we still reward it.
                        score += f.Fiber * fiberMultiplier;
                    }

                    // 2. Calorie Penalty: You are over on calories, so we heavily penalize high-calorie foods.
                    if (remainingCalories <= 0)
                    {
                        score -= f.TotalCalories * calorieMultiplier;
                    }
                    else
                    {
                        // If you were under, this would be the logic to reward low-calorie foods
                        score -= f.TotalCalories * calorieMultiplier;
                    }

                    // 3. Protein Penalty: You are over on protein.
                    if (remainingProtein <= 0)
                    {
                        score -= f.Protein * proteinMultiplier;
                    }
                    else
                    {
                        score += Math.Min(f.Protein, remainingProtein) * proteinMultiplier;
                    }

                    // 4. Fat Penalty: You are over on fat.
                    if (remainingFat <= 0)
                    {
                        score -= f.Fats * fatMultiplier;
                    }
                    else
                    {
                        score -= f.Fats * fatMultiplier;
                    }

                    // 5. Carb Penalty: You are over on carbs.
                    if (remainingCarbs <= 0)
                    {
                        score -= f.Carbs * carbMultiplier;
                    }
                    else
                    {
                        score -= f.Carbs * carbMultiplier;
                    }

                    return score;
                });
        }
    }
}
