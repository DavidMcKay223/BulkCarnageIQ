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

        public float PortionEaten { get; set; } = 1f;
        public float? MeasurementServings { get; set; }  // e.g., 0.25
        public string? MeasurementType { get; set; }     // e.g., "Cup", "Part", "Ounces", etc.

        // Optional nutrition info per portion
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fats { get; set; }
        public float Fiber { get; set; }

        public required string UserId { get; set; } // User ID for the meal entry
    }
}
