using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data.Services
{
    public class LogService
    {

        private ApplicationDbContext database;
        public LogService(ApplicationDbContext database)
        {
            this.database = database;
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            Log log = new Log()
            {
                CreatedDateTime = DateTime.Now,
                Description = "This is a test log",
                Type = "Test",
                Archived = false,
            };

            database.Add(log);

            return await database.Logs.OrderByDescending(x => x.CreatedDateTime).ToListAsync();
        }
    }
}
