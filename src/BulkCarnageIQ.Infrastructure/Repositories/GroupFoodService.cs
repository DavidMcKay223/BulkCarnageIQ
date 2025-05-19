using BulkCarnageIQ.Core.Carnage;
using BulkCarnageIQ.Core.Contracts;
using BulkCarnageIQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Infrastructure.Repositories
{
    public class GroupFoodService : IGroupFoodService
    {
        private readonly AppDbContext _db;

        public GroupFoodService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroupFoodItem>> GetAllAsync()
        {
            return await _db.GroupFoodItems
                .Include(g => g.Entries)
                .ToListAsync();
        }

        public async Task<List<FoodItem>> GetFoodItemByGroupName(string groupName)
        {
            var group = await _db.GroupFoodItems
                .Include(g => g.Entries)
                .FirstOrDefaultAsync(g => g.GroupName == groupName);

            if (group == null || group.Entries.Count == 0)
            {
                return new List<FoodItem>();
            }

            var recipeNames = group.Entries.Select(e => e.RecipeName).ToList();

            return await _db.FoodItems
                .Where(f => f.RecipeName.Equals(recipeNames))
                .ToListAsync();
        }

        public async Task<List<string>> GetGroupNames()
        {
            return await _db.GroupFoodItems
                .Select(f => f.GroupName)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
        }
    }
}
