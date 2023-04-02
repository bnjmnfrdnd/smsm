using Azure.Identity;
using System.ComponentModel;

namespace smsm.Data.Models
{
    public class Option : BaseModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Other { get; set; }       
    }
}
