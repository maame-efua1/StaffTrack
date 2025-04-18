﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffTrack.API.Models;

namespace StaffTrack.API.Data
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
