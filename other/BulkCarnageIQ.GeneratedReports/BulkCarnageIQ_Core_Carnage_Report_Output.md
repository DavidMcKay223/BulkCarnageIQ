# Directory: Carnage\Report

## File: FoodItemFilter.cs

```C#
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
        public bool IsLowFiber { get; set; }
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
            IsLowFiber = false;
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
                (string.IsNullOrEmpty(SelectedRecipeName) || item.RecipeName == SelectedRecipeName) &&
                (string.IsNullOrEmpty(SelectedBrandType) || item.BrandType == SelectedBrandType) &&
                (string.IsNullOrEmpty(SelectedGroupName) || item.GroupName == SelectedGroupName) &&
                (!IsHighProtein || item.IsHighProtein) &&
                (!IsLowCarb || item.IsLowCarb) &&
                (!IsKeto || item.IsKeto) &&
                (!IsBulkMeal || item.IsBulkMeal) &&
                (!IsHighCarb || item.IsHighCarb) &&
                (!IsLowFiber || item.IsLowFiber) &&
                (!IsHighFat || item.IsHighFat) &&
                (!IsBalancedMeal || item.IsBalancedMeal) &&
                (!IsLowProtein || item.IsLowProtein)
            );
        }
    }
}

```

## File: MacroSummary.cs

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Report
{
    public class MacroSummary
    {
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Fiber { get; set; }
    }
}

```

## File: WeightProjection.cs

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Report
{
    public class WeightProjection
    {
        public float CurrentWeight { get; set; }
        public float ProjectedWeight7Days { get; set; }
        public float ProjectedWeight14Days { get; set; }
    }
}

```

