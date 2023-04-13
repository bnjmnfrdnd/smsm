using Microsoft.EntityFrameworkCore;
using smsm.Data.Migrations;
using smsm.Data.Models;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace smsm.Data.Services
{
    public class ContentService
    {
        private LogService logService;
        private ApplicationDbContext database;
        private IMDbService IMDbService;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public ContentService(ApplicationDbContext database, LogService logService, IMDbService IMDbService)
        {
            this.database = database;
            this.logService = logService;
            this.IMDbService = IMDbService;
        }

        public async Task<List<Content>> UploadFileAsync(string file)
        {
            List<string> data = file.Split("\n").ToList();
            List<string> contentList = new List<string>();
            List<string> removables = new List<string>() { ".mkv", ".mp4", ".wmv", ".mov", ".avi", ".MKV", ".MP4", ".WMV", ".MOV", ".AVI", "\r" };
            List<string> skipList = new List<string>() { "Name", "", "----" };
            string apiKey = database.Options.FirstOrDefault(x => x.Type == "IMDB_API_KEY")?.Value;

            foreach (string content in data)
            {
                string newContent = content.Trim();
                removables.ForEach(removable => newContent = newContent.Replace(removable, ""));
                contentList.Add(newContent);
            }

            try
            {
                foreach (string content in contentList)
                {
                    if (!skipList.Any(x => x.ToUpper().Equals(content.ToUpper())))
                    {
                        var year = Regex.Match(content, @"\(([^)]*)\)").Groups[1].Value;
                        var title = Regex.Replace(content, @"\((.*?)\)", "").Trim();

                        if (!Int32.TryParse(year, out var year_int))
                        {
                            year = String.Empty;
                        }
                        else
                        {
                            if (Int32.Parse(year) < 1900)
                            {
                                year = String.Empty;
                            }
                        }

                        Content newContent = new Content()
                        {
                            Title = title.ToString(),
                            Year = year,
                            CreatedDateTime = DateTime.Now,
                            Type = year != String.Empty ? "Movie" : "TV Series",
                            ImdbId = String.Empty,
                            Comments = String.Empty,
                            Description = String.Empty,
                            Archived = false,
                        };

                        //if (apiKey != null)
                        //{
                        //    IMDb iMDb = new IMDb();
                        //    iMDb = await IMDbService.SearchTitleAsync(newContent.Title);

                        //    if (iMDb.results != null)
                        //    {
                        //        newContent.ImdbId = iMDb.results[0].id.ToUpper();
                        //        newContent.Title = iMDb.results[0].title;
                        //        newContent.Description = iMDb.results[0].description;
                        //    }
                        //}

                        database.Add(newContent);

                        logService.CreateLog($"{newContent.Type} Added (Upload)", $"{newContent.Title} ({newContent.Year})");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            await database.SaveChangesAsync();

            return await GetContentAsync();
        }

        public async Task<List<Content>> GetContentDashboardAsync()
        {
            return await database.Content.Where(x => !x.Archived).OrderBy(x => x.Title).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> GetContentRequestsDashboardAsync()
        {
            return await database.ContentRequests.Where(x => !x.Archived).OrderBy(x => x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<Content>> GetContentAsync()
        {
            return await database.Content.Where(x => !x.Archived).OrderBy(x => x.Title).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> GetContentRequestsAsync()
        {
            return await database.ContentRequests.Where(x => !x.Archived).OrderBy(x => x.IsComplete).ThenByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task<List<ContentRequest>> SaveContentRequest(ContentRequest contentRequest)
        {
            try
            {
                string apiKey = database.Options.FirstOrDefault(x => x.Type == "IMDB_API_KEY")?.Value;
                if (contentRequest.Title == "" && contentRequest.Title == null)
                {
                    return await GetContentRequestsAsync();
                }
                IMDb iMDb = new IMDb();

                contentRequest.Title = textInfo.ToTitleCase(contentRequest.Title);

                if (apiKey != null)
                {
                    iMDb = await IMDbService.SearchTitleAsync(contentRequest.Title);

                    if (iMDb.results != null && iMDb.results.Count != 0)
                    {
                        contentRequest.ImdbId = iMDb.results[0].id != null ? iMDb.results[0].id.ToUpper() : String.Empty;
                        contentRequest.Title = iMDb.results[0].title != null ? contentRequest.Title : String.Empty;
                        contentRequest.Comments = contentRequest.Comments != null ? contentRequest.Comments : iMDb.results[0].description;
                    }
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
                    logService.CreateLog($"{contentRequest.Type} Request Added", $"{contentRequest.Title} ({contentRequest.Year})");
                }
                else
                {
                    database.Update(contentRequest);
                    var existingContentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == contentRequest.Id);
                    logService.CreateLog($"{contentRequest.Type} Request Updated", $"{contentRequest.Title} ({contentRequest.Year})");
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
                List<Content> contentList = database.Content.Where(x => !x.Archived).ToList();

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

                    foreach (Content c in contentList)
                    {
                        if (
                            (c.Title.ToUpper().Trim() == content.Title.ToUpper().Trim()
                            && c.Type.ToUpper().Trim() == content.Type.ToUpper().Trim())
                            && c.ImdbId == content.ImdbId
                            )
                        {
                            return await GetContentAsync();
                        }
                    }

                    database.Add(content);
                    logService.CreateLog($"{content.Type} Added", $"{content.Title} ({content.Year})");
                }
                else
                {
                    database.Update(content);
                    var existingContent = database.Content.SingleOrDefault(x => x.Id == content.Id);
                    logService.CreateLog($"{content.Type} Updated", $"{content.Title} ({content.Year})");
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

            logService.CreateLog($"{content.Type} Deleted", $"{content.Title} ({content.Year})");

            return await GetContentAsync();
        }

        public async Task<List<ContentRequest>> ChangeContentRequestStatus(int id)
        {
            Content content = null;
            ContentRequest contentRequest = database.ContentRequests.SingleOrDefault(x => x.Id == id);

            if (contentRequest == null)
            {
                return await GetContentRequestsAsync();
            }
            contentRequest.IsComplete = !contentRequest.IsComplete;

            if (contentRequest.IsComplete)
            {
                logService.CreateLog("Content Request Complete", $"{contentRequest.Title} ({contentRequest.Year})");

                content = new Content()
                {
                    Title = contentRequest.Title,
                    Type = contentRequest.Type,
                    Year = contentRequest.Year,
                    ImdbId = contentRequest.ImdbId,
                };
            }
            else
            {
                logService.CreateLog("Content Request Incomplete", $"{contentRequest.Title} ({contentRequest.Year}) ");
            }

            database.Update(contentRequest);
            await database.SaveChangesAsync();

            if (content != null)
            {
                await SaveContent(content);
            }

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