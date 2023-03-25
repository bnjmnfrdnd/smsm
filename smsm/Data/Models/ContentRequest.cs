using Azure.Identity;
using System.ComponentModel;

namespace smsm.Data.Models
{
    public class ContentRequest : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Year { get; set; }
        public string? ImdbId { get; set; }
        public string Type { get; set; }
        public string? Comments { get; set; }
        public bool IsComplete { get; set; }
        public int UserId { get; set; }
        public virtual string UserName { get;set; }
    }
}
