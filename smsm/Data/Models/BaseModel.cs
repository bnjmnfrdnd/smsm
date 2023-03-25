namespace smsm.Data.Models
{
    public abstract class BaseModel
    {
        public DateTime CreatedDateTime { get; set; }

        public bool Archived { get; set; }
    }
}
