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

        public bool IsFavorite { get; set; } = false;     // Pinned or quick-add item
        public bool IsCompleted { get; set; } = false;    // Checked off in the UI

        public string? MeasurementType { get; set; }      // e.g., "Cup", "Ounces", "Pcs"
        public float? MeasurementServings { get; set; }   // e.g., 2.5 servings

        [Required]
        public string UserId { get; set; } = string.Empty; // For multi-user support

        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Just the date, no time
    }
}
