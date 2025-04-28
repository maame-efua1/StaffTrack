using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffTrackAPI.Models;

namespace StaffTrackAPI.Data
{
    public class StaffTrackDbContext : IdentityDbContext<User>
    {
        public StaffTrackDbContext(DbContextOptions<StaffTrackDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Report> Reports { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                   .Property(u => u.StaffStatus)
                   .HasConversion<string>();
        }
    }
}
