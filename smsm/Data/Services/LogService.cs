using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data.Services
{
    public class LogService
    {

        private ApplicationDbContext database;
        UserManager<IdentityUser> userManager;

        public LogService(ApplicationDbContext database, UserManager<IdentityUser> userManager)
        {
            this.database = database;
            this.userManager = userManager;
            var currentUserId = this.userManager.GetUserIdAsync;
        }

        public async Task<List<Log>> GetLogsAsync()
        {           
            return await database.Logs.OrderByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public void CreateLog(string type, string description)
        {
            var currentUserId = this.userManager.GetUserIdAsync;

            Log log = new Log()
            {
                Type = type,
                Description = description,
                CreatedDateTime = DateTime.Now,
                Archived = false,
            };

            database.Add(log);
            database.SaveChanges();
        }
    }
}
