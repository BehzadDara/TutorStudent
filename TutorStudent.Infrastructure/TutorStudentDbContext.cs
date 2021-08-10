using System;
using Microsoft.EntityFrameworkCore;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Models;
using TutorStudent.Infrastructure.Implementations;

namespace TutorStudent.Infrastructure
{
    public class TutorStudentDbContext : DbContextBase
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Student> Students { get; set; }
        
        public TutorStudentDbContext(DbContextOptions options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<Tutor>().HasKey(x => x.Id);
            modelBuilder.Entity<Student>().HasKey(x => x.Id);
            
            modelBuilder.Entity<User>().Property(e => e.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => (GenderType)Enum.Parse(typeof(GenderType), v));  
            
            modelBuilder.Entity<User>().Property(e => e.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (RoleType)Enum.Parse(typeof(RoleType), v));


        }
    }
}