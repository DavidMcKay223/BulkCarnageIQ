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
    }
}
