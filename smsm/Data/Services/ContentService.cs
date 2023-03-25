using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data.Services
{
    public class ContentService
    {
        private ApplicationDbContext database;
        private LogService logService;

        public ContentService(ApplicationDbContext database, LogService logService)
        {
            this.database = database;
            this.logService = logService;
        }

        public async Task<List<ContentRequest>> GetContentRequestsAsync()
        {            
            return await database.ContentRequests.Where(x => !x.Archived).OrderBy(x=> x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> SaveContentRequest(ContentRequest contentRequest)
        {
            try
            {
                if (contentRequest.Id == 0)
                {
                    contentRequest.CreatedDateTime = DateTime.Now;
                    contentRequest.Archived = false;
                    contentRequest.Type = contentRequest.Type == null ? "" : contentRequest.Type;
                    contentRequest.Title = contentRequest.Title == null ? "" : contentRequest.Title;
                    contentRequest.Year = contentRequest.Year == null ? "" : contentRequest.Year;
                    contentRequest.ImdbId = contentRequest.ImdbId == null ? "" : contentRequest.ImdbId;
                    contentRequest.UserName = "";
                    contentRequest.UserId = 0;

                    database.Add(contentRequest);
                    logService.CreateLog("New Content Request", $"Request for {contentRequest.Type} titled '{contentRequest.Title}' from {contentRequest.Year}.");
                }
                else
                {
                    database.Update(contentRequest);
                    var existingContentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == contentRequest.Id);
                    logService.CreateLog("Content Request Updated", $"Request for {contentRequest.Type} titled '{contentRequest.Title}' from {contentRequest.Year}.");
                }

                await database.SaveChangesAsync();


                return await GetContentRequestsAsync();
            }
            catch (Exception)
            {
                return await GetContentRequestsAsync();
            }
        }

        public async Task<List<ContentRequest>> ChangeContentRequestStatus(int id)
        {
            var contentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == id);
            contentRequest.IsComplete = !contentRequest.IsComplete;

            if (contentRequest.IsComplete)
            {
                logService.CreateLog("Content Request Complete", $"Request for {contentRequest.Type} titled '{contentRequest.Title}' from {contentRequest.Year} marked as completed.");
            }
            else
            {
                logService.CreateLog("Content Request Incomplete", $"Request for {contentRequest.Type} titled '{contentRequest.Title}' from {contentRequest.Year} marked as incompleted.");
            }

            database.Update(contentRequest);
            await database.SaveChangesAsync();

            return await GetContentRequestsAsync();
        }    

        public async Task<List<ContentRequest>> DeleteContentRequest(ContentRequest contentRequest)
        {
            contentRequest.Archived = true;

            database.Update(contentRequest);
            await database.SaveChangesAsync();

            logService.CreateLog("Content Request Deleted", $"Request for {contentRequest.Type} titled '{contentRequest.Title}' from {contentRequest.Year} marked as deleted.");

            return await GetContentRequestsAsync();
        }        
        

    }
}
