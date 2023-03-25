namespace smsm.Data.Models
{
    public class Log : BaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
