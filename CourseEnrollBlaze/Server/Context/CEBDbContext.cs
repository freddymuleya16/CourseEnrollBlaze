using CourseEnrollBlaze.Shared.Entities;
using CourseEnrollBlaze.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CourseEnrollBlaze.Server.Context
{
    public class CEBDbContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public CEBDbContext(DbContextOptions<CEBDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<Course>().Property(c => c.Id).ValueGeneratedOnAdd();
                  

            modelBuilder.Entity<Course>().HasKey(u => u.Id);

            modelBuilder.Entity<UserAccount>().HasKey(u => u.Id);
            modelBuilder.Entity<Notification>().HasKey(n => n.Id);

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.UserId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
