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
