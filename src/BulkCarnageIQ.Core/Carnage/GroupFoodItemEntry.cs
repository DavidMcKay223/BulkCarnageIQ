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
