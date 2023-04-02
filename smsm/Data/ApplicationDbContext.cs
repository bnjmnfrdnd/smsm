
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using smsm.Data.Models;

namespace smsm.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=smsm.db");
        }

        public virtual DbSet<Log> Logs { get; set; }

        public virtual DbSet<Content> Content { get; set; }

        public virtual DbSet<ContentRequest> ContentRequests { get; set; }

        public virtual DbSet<Option> Options { get; set; }

        public virtual DbSet<IdentityUser> Users { get; set; }
    }
}