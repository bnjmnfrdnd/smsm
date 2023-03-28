using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Migrations;
using smsm.Data.Models;
using System.Globalization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace smsm.Data.Services
{
    public class ContentService
    {
        private ApplicationDbContext database;
        private LogService logService;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public ContentService(ApplicationDbContext database, LogService logService)
        {
            this.database = database;
            this.logService = logService;
        }

        public async Task<List<Content>> UploadFileAsync(string file)
        {
            file = file.Trim();
            file = file.Replace("\r\n", ",");
            file = file.Replace(".png", ",");
            file = file.Replace(".mkv", ",");
            file = file.Replace(".mp4", ",");
            file = file.Replace(".wav", ",");
            file = file.Replace(".mp3", ",");
            file = file.Replace(".m4a", ",");
            

            while (file.Contains(",,"))
            {
                file = file.Replace(",,", ",");
            }
            while (file.Contains("  "))
            {
                file = file.Replace("  ", " ");
            }
            while (file.Contains(", ,"))
            {
                file = file.Replace(", ,", ",");
            }

            List<string> contentList = file.Split(',').ToList();
            List<string> duplicateContentList = new List<string>();

            foreach(string str in contentList)
            {
                switch (str)
                {
                    case " ":
                        break;
                    case "":
                        break;
                    case "Name ":
                        break;
                    case "---- ":
                        break;
                    default:
                        duplicateContentList.Add(str);
                        break;
                }
            }

            foreach (string str in duplicateContentList)
            {

                var year = Regex.Matches(str, @"\[(.*?)\]");
                var title = Regex.Replace(str, "\\[([^\\s]*)\\]", "$1");

                Content content = new Content()
                {
                    Title = title,
                    Year = year.Count != 0 ? year[0].ToString() : "N/A",
                    CreatedDateTime = DateTime.Now,
                    Type = year.Count != 0 ? "Movie" : "Uploaded",
                };

                database.Add(content);
            }
            await database.SaveChangesAsync();

            return await database.Content.Where(x => !x.Archived).OrderBy(x => x.Title).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<Content>> GetContentAsync()
        {
            return await database.Content.Where(x => !x.Archived).OrderBy(x => x.Title).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> GetContentRequestsAsync()
        {            
            return await database.ContentRequests.Where(x => !x.Archived).OrderBy(x=> x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> SaveContentRequest(ContentRequest contentRequest)
        {
            try
            {
                if (contentRequest.Title != "" && contentRequest.Title != null)
                {
                    contentRequest.Title = textInfo.ToTitleCase(contentRequest.Title);
                }

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
                    logService.CreateLog("Content Request Added", $"{contentRequest.Title} {contentRequest.Year} - {contentRequest.Type}");
                }
                else
                {
                    database.Update(contentRequest);
                    var existingContentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == contentRequest.Id);
                    logService.CreateLog("Content Request Updated", $"{contentRequest.Title} {contentRequest.Year} - {contentRequest.Type}");
                }

                await database.SaveChangesAsync();


                return await GetContentRequestsAsync();
            }
            catch (Exception)
            {
                return await GetContentRequestsAsync();
            }
        }

        public async Task<List<Content>> SaveContent(Content content)
        {
            try
            {
                if (content.Title != "" && content.Title != null)
                {
                    content.Title = textInfo.ToTitleCase(content.Title);
                }

                if (content.Id == 0)
                {
                    content.CreatedDateTime = DateTime.Now;
                    content.Archived = false;
                    content.Type = content.Type == null ? "" : content.Type;
                    content.Title = content.Title == null ? "" : content.Title;
                    content.Year = content.Year == null ? "" : content.Year;
                    content.ImdbId = content.ImdbId == null ? "" : content.ImdbId;

                    database.Add(content);
                    logService.CreateLog($"{content.Type} Added", $"{content.Title} ({content.Year}).");
                }
                else
                {
                    database.Update(content);
                    var existingContent = database.Content.SingleOrDefault(x => x.Id == content.Id);
                    logService.CreateLog($"{content.Type} Updated", $"{content.Title} ({content.Year}).");
                }

                await database.SaveChangesAsync();


                return await GetContentAsync();
            }
            catch (Exception)
            {
                return await GetContentAsync();
            }
        }

        public async Task<List<Content>> DeleteContent(Content content)
        {
            content.Archived = true;

            database.Update(content);
            await database.SaveChangesAsync();

            logService.CreateLog($"{content.Type} Deleted", $"{content.Title} ({content.Year}) - {content.Type}.");

            return await GetContentAsync();
        }

        public async Task<List<ContentRequest>> ChangeContentRequestStatus(int id)
        {
            var contentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == id);
            contentRequest.IsComplete = !contentRequest.IsComplete;

            if (contentRequest.IsComplete)
            {
                logService.CreateLog("Content Request Complete", $"{contentRequest.Title} {contentRequest.Year} - {contentRequest.Type}");
            }
            else
            {
                logService.CreateLog("Content Request Incomplete", $"{contentRequest.Title} {contentRequest.Year} - {contentRequest.Type}");
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

            logService.CreateLog("Content Request Deleted", $"{contentRequest.Title} {contentRequest.Year} - {contentRequest.Type}");

            return await GetContentRequestsAsync();
        }        
    }
}
