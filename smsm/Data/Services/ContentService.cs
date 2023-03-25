using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data.Services
{
    public class ContentService
    {
        private ApplicationDbContext database;

        public ContentService(ApplicationDbContext database)
        {
            this.database = database;
        }

        public async Task<List<ContentRequest>> GetContentRequestsAsync()
        {
            ContentRequest contentRequest = new ContentRequest()
            {
                Title = "Harry Potter and the Philiosophers Stone",
                Year = "2001",
                ImdbId = "tt0241527",
                CreatedDateTime = DateTime.Now,
                Type = "Movie",
                Comments = "Also known as Harry Potter and the Sorcerer's Stone",
                IsComplete = true,
                Archived = false,
            };



            return await database.ContentRequests.OrderBy(x=> x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> SaveContentRequest(ContentRequest contentRequest)
        {
            try
            {
                contentRequest.CreatedDateTime = DateTime.Now;
                contentRequest.Archived = false;

                database.Add(contentRequest);
                database.SaveChangesAsync();

                return await database.ContentRequests.OrderBy(x => x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
            }
            catch (Exception ex)
            {
                return await database.ContentRequests.OrderBy(x => x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
            }
        }
    }
}
