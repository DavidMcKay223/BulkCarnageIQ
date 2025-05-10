using BulkCarnageIQ.Core.Carnage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Contracts
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfile(string userName);
        Task SaveUserProfile(UserProfile userProfile);
        Task UpdateUserGoals(string userName);
    }
}
