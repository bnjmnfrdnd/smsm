using Azure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;
using System.Security.Claims;
using System.Web;

namespace smsm.Data.Services
{
    public class UserService
    {
        private LogService logService;
        private ApplicationDbContext database;
        private UserManager<IdentityUser> userManager;

        public UserService(ApplicationDbContext database, UserManager<IdentityUser> userManager, LogService logService)
        {
            this.database = database;
            this.userManager = userManager;
            this.logService = logService;
        }

        public async Task<List<IdentityUser>> GetUsersAsync()
        {
            return await database.Users.ToListAsync();
        }

        public async Task<List<IdentityUser>> DeleteUser(IdentityUser user)
        {
            database.Remove(user);
            await database.SaveChangesAsync();
            return await GetUsersAsync();
        }
    }
}
