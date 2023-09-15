using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DbSet<TokenProvider> TokenProviders { get; set; }    

        public DbSet<Email> Emails { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectTask> Tasks { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }
    }
}
