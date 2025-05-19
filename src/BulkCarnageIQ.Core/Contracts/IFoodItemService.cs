using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IFoodItemService
    {
        Task<FoodItem?> GetFoodItemByName(string recipeName);
        Task<List<FoodItem>> GetAllAsync();
        Task<Dictionary<string, FoodItem>> GetAllDictionaryAsync(List<string> recipeName);
    }
}
