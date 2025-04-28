using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IGroupFoodService
    {
        Task<List<FoodItem>> GetFoodItemByGroupName(string groupName);
        Task<List<string>> GetGroupNames();
    }
}
