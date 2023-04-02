using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;
using System.Net.Http.Headers;

namespace smsm.Data.Services
{
    public class IMDbService
    {
        private LogService logService;
        private ApplicationDbContext database;
        private UserManager<IdentityUser> userManager;
        private string path = "";
        private string apiKey = "";
        static HttpClient client = new HttpClient();

        public IMDbService(ApplicationDbContext database, UserManager<IdentityUser> userManager, LogService logService)
        {
            this.database = database;
            this.userManager = userManager;
            this.logService = logService;
            var currentUserId = this.userManager.GetUserIdAsync;
            apiKey = database.Options.FirstOrDefault(x => x.Type == "IMDB_API_KEY")?.Value;
        }

        static async Task SetupHttpClient()
        {
            client.BaseAddress = new Uri("https://imdb-api.com/en/API/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IMDb> SearchTitleAsync(string title)
        {
            SetupHttpClient();
            path = $"{client.BaseAddress}SearchTitle/{apiKey}/{title}";    
            IMDb iMDb = new IMDb();
            HttpResponseMessage response = await client.GetAsync(path);
            
            if (response.IsSuccessStatusCode)
            {
                iMDb = await response.Content.ReadFromJsonAsync<IMDb>();
            }

            return iMDb;
        }
    }
}