# Directory: Carnage

## File: FoodItem.cs

```C#
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
        public bool IsHighCarb => Carbs >= 25f && Fiber < 3f;
        public bool IsLowFiber => Fiber < 2f;
        public bool IsHighFat => Fats >= 20f;
        public bool IsBalancedMeal => Protein >= 15f && Carbs <= 60f && Fats <= 20f;
        public bool IsLowProtein => Protein < 5f;
    }
}

```

## File: GroceryListItem.cs

```C#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class GroceryListItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RecipeName { get; set; } = string.Empty; // Matches FoodItem.RecipeName

        public string? GroupName { get; set; }

        public bool IsFavorite { get; set; } = false;     // Pinned or quick-add item
        public bool IsCompleted { get; set; } = false;    // Checked off in the UI

        public string? MeasurementType { get; set; }      // e.g., "Cup", "Ounces", "Pcs"
        public float? MeasurementServings { get; set; }   // e.g., 2.5 servings

        [Required]
        public string UserId { get; set; } = string.Empty; // For multi-user support

        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Just the date, no time
    }
}

```

## File: GroupFoodItem.cs

```C#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class GroupFoodItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string GroupName { get; set; }

        public string? Description { get; set; }

        // Navigation property: A group has many entries
        public List<GroupFoodItemEntry> Entries { get; set; } = new ();
    }
}

```

## File: GroupFoodItemEntry.cs

```C#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class GroupFoodItemEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupFoodItemId { get; set; }

        [ForeignKey(nameof(GroupFoodItemId))]
        public required GroupFoodItem GroupFoodItem { get; set; }

        [Required]
        public required string RecipeName { get; set; }

        [Required]
        public float PortionAmount { get; set; }
    }
}

```

## File: MealEntry.cs

```C#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class MealEntry
    {
        [Key]
        public int Id { get; set; }

        public required DateOnly Date { get; set; }   // Date of the meal entry, e.g., 2023-10-01
        public required string Day { get; set; }      // "Monday", "Tuesday", etc.
        public required string MealType { get; set; } // "Breakfast", "Lunch", etc.
        public required string MealName { get; set; } // Must not be empty to save
        public string? GroupName { get; set; }

        public float PortionEaten { get; set; } = 1f;
        public float? MeasurementServings { get; set; }  // e.g., 0.25
        public string? MeasurementType { get; set; }     // e.g., "Cup", "Part", "Ounces", etc.

        // Optional nutrition info per portion
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Fiber { get; set; }

        // Flags based on macros
        public bool IsHighProtein => Protein >= 20f;
        public bool IsLowCarb => Carbs < 10f;
        public bool IsKeto => Fats > (Carbs * 3f);
        public bool IsBulkMeal => Calories > 600f;
        public bool IsHighCarb => Carbs >= 25f && Fiber < 3f;
        public bool IsLowFiber => Fiber < 2f;
        public bool IsHighFat => Fats >= 20f;
        public bool IsLowProtein => Protein < 5f;
        public bool IsBalancedMeal => Protein >= 15f && Carbs <= 60f && Fats <= 20f;

        public required string UserId { get; set; } // User ID for the meal entry
    }
}

```

## File: UserProfile.cs

```C#
using BulkCarnageIQ.Core.Carnage.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class UserProfile
    {
        [Key]
        [Required]
        public required string UserName { get; set; }

        public float Age { get; set; }
        public Sex Sex { get; set; }
        public float HeightInches { get; set; }
        public float WeightPounds { get; set; }
        public required string ActivityLevel { get; set; }
        public required string GoalType { get; set; }

        // Calculated Goals
        public float CalorieGoal { get; set; }
        public float ProteinGoal { get; set; }
        public float CarbsGoal { get; set; }
        public float FatGoal { get; set; }
        public float FiberGoal { get; set; }

        public void CalculateGoals()
        {
            float bmr = CalculateBMR();
            float tdee = bmr * GetActivityFactor();
            float adjustedCalories = ApplyGoalAdjustment(tdee);

            CalorieGoal = adjustedCalories;

            float weightKg = PoundsToKg(WeightPounds);

            // Protein goal calculation - typically grams per kg.
            ProteinGoal = Math.Min(weightKg * 1.5f, 175f);

            // Fat goal calculation as a percentage of calories
            FatGoal = CalorieGoal * 0.25f / 9f;

            // Carb goal calculation based on remaining calories
            CarbsGoal = (CalorieGoal - (ProteinGoal * 4f + FatGoal * 9f)) / 4f;

            // Fiber goal calculation based on calories (14g per 1000 kcal)
            FiberGoal = (CalorieGoal / 1000.0f) * 14.0f;
        }

        private float CalculateBMR()
        {
            float weightKg = PoundsToKg(WeightPounds);
            float heightCm = InchesToCm(HeightInches);

            float bmr;

            if (Sex == Sex.Male)
            {
                // Mifflin-St Jeor for males
                bmr = 10f * weightKg + 6.25f * heightCm - 5f * Age + 5f;
            }
            else
            {
                // Mifflin-St Jeor for females
                bmr = 10f * weightKg + 6.25f * heightCm - 5f * Age - 161f;
            }

            return bmr;
        }

        private float GetActivityFactor()
        {
            switch (ActivityLevel.ToLower())
            {
                case "sedentary": return 1.2f;
                case "lightly active": return 1.375f;
                case "moderately active": return 1.55f;
                case "very active": return 1.725f;
                case "super active": return 1.9f;
                default: return 1.2f; // Default to sedentary if level is unknown
            }
        }

        private float ApplyGoalAdjustment(float tdee)
        {
            switch (GoalType.ToLower())
            {
                case "maintain":
                    return tdee;
                case "lose0.5": // Lose 0.5 lbs/week (approx 250 kcal deficit/day)
                    return tdee - 250f;
                case "lose1": // Lose 1 lbs/week (approx 500 kcal deficit/day)
                    return tdee - 500f;
                case "lose2": // Lose 2 lbs/week (approx 1000 kcal deficit/day)
                    return tdee - 1000f;
                case "gain0.5": // Gain 0.5 lbs/week (approx 250 kcal surplus/day)
                    return tdee + 250f;
                case "gain1": // Gain 1 lbs/week (approx 500 kcal surplus/day)
                    return tdee + 500f;
                default:
                    return tdee; // Default to maintain if goal is unknown
            }
        }

        private float PoundsToKg(float pounds) => pounds * 0.453592f;
        private float InchesToCm(float inches) => inches * 2.54f;
    }
}

```

## File: WeightLog.cs

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class WeightLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateOnly Date { get; set; }
        public float WeightLbs { get; set; }
    }
}

```

