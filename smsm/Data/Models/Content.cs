namespace smsm.Data.Models
{
    public class Content : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Comments { get; set; }
        public string? ImdbId { get; set; }
        public string Year { get; set; }
        public string Type { get; set; }
    }
}
