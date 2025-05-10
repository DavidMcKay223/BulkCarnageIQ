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
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _db;

        public UserProfileService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfile> GetUserProfile(string userName)
        {
            var profile = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.UserName == userName);

            return profile ?? new UserProfile
            {
                UserName = userName,
                ActivityLevel = "sedentary",
                GoalType = "maintain",
                Age = 25f,
                ProteinGoal = 120f,
                CarbsGoal = 220f,
                FatGoal = 80f,
                FiberGoal = 25f,
                CalorieGoal = 2800f,
            };
        }

        public async Task SaveUserProfile(UserProfile userProfile)
        {
            var existing = await _db.UserProfiles
                .FindAsync(userProfile.UserName);

            if (existing == null)
            {
                await _db.UserProfiles.AddAsync(userProfile);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(userProfile);
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateUserGoals(string userName)
        {
            var userProfile = await GetUserProfile(userName);
            userProfile.CalculateGoals();
            await SaveUserProfile(userProfile);
        }
    }
}
