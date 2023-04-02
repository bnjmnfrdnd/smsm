using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data.Services
{
    public class OptionService
    {
        private LogService logService;
        private ApplicationDbContext database;
        private UserManager<IdentityUser> userManager;

        public OptionService(ApplicationDbContext database, UserManager<IdentityUser> userManager, LogService logService)
        {
            this.database = database;
            this.userManager = userManager;
            this.logService = logService;
            var currentUserId = this.userManager.GetUserIdAsync;
        }

        public async Task<List<Option>> GetOptionsAsync()
        {           
            return await database.Options.ToListAsync();
        }

        public async Task<List<Option>> SaveOptionAsync(string type, string value)
        {
            Option oldOption = database.Options.FirstOrDefault(x => x.Type == type);

            Option option = new Option 
            { 
                Type = type, 
                Value = value,
                CreatedDateTime = DateTime.Now,
                Other = String.Empty
            };

            if (oldOption != null)
            {
                database.Remove(oldOption);
            }

            await database.AddAsync(option);
            await database.SaveChangesAsync();
            logService.CreateLog($"{type} Updated", $"{oldOption?.Value ?? ""} changed to {option.Value}");

            return await GetOptionsAsync();
        }
    }
}
