using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Engine
{
    public class FoodItemSuggestionGroup
    {
        public required String GroupName { get; set; }
        public List<FoodItemSuggestion> Items { get; set; } = new List<FoodItemSuggestion>();
    }
}
