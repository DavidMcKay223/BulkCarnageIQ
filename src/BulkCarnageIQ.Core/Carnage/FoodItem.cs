using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class FoodItem
    {
        [Key]
        public required string RecipeName { get; set; } // Unique key, e.g., "Banana" or "Double Double Protein Style"

        public string BrandType { get; set; } = "Generic"; // "Generic" or "Brand"

        public string? GroupName { get; set; }

        public float TotalCalories => Servings * CaloriesPerServing;

        public float Servings { get; set; }             // Number of servings
        public float CaloriesPerServing { get; set; }

        public float MeasurementServings { get; set; }  // e.g., 0.25
        public required string MeasurementType { get; set; }     // e.g., "Cup", "Part", "Ounces", etc.

        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Fiber { get; set; }

        // Meal flags — super easy to filter
        public bool IsBreakfast { get; set; }
        public bool IsLunch { get; set; }
        public bool IsDinner { get; set; }
        public bool IsSnack { get; set; }

        public string? Link { get; set; } // Optional, nutrition or source URL
        public string? PictureLink { get; set; } // Optional, picture URL

        // Derived nutrition flags (positive and risk)
        public bool IsHighProtein => Protein >= 20f;
        public bool IsLowCarb => Carbs < 10f;
        public bool IsKeto => Fats > (Carbs * 3f);
        public bool IsBulkMeal => TotalCalories > 600f;
        public bool IsHighSugar => Carbs >= 25f && Fiber < 3f;
        public bool IsLowFiber => Fiber < 2f;
        public bool IsHighFat => Fats >= 20f;
        public bool IsCalorieDense => TotalCalories >= 400f && Servings < 1f;
        public bool IsLowProtein => Protein < 5f;
    }
}
