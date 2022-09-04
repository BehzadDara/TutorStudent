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
        public DbSet<TutorSchedule> TutorSchedules { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Apply> Applies { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<TutorWeeklySchedule> TutorWeeklySchedules { get; set; }
        public DbSet<TeacherAssistant> TeacherAssistants { get; set; }
        public DbSet<TeacherAssistantSchedule> TeacherAssistantSchedules { get; set; }
        public DbSet<TeacherAssistantMeeting> TeacherAssistantMeetings { get; set; }
        public DbSet<FacultyManagementSuggestion> FacultyManagementSuggestions { get; set; }
        public DbSet<FacultyManagementSuggestionTutor> FacultyManagementSuggestionTutors { get; set; }
        
        public TutorStudentDbContext(DbContextOptions options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<Tutor>().HasKey(x => x.Id);
            modelBuilder.Entity<Student>().HasKey(x => x.Id);
            modelBuilder.Entity<TutorSchedule>().HasKey(x => x.Id);
            modelBuilder.Entity<Meeting>().HasKey(x => x.Id);
            modelBuilder.Entity<Advertisement>().HasKey(x => x.Id);
            modelBuilder.Entity<Apply>().HasKey(x => x.Id);
            modelBuilder.Entity<Log>().HasKey(x => x.Id);
            modelBuilder.Entity<TutorWeeklySchedule>().HasKey(x => x.Id);
            modelBuilder.Entity<TeacherAssistant>().HasKey(x => x.Id);
            modelBuilder.Entity<TeacherAssistantSchedule>().HasKey(x => x.Id);
            modelBuilder.Entity<TeacherAssistantMeeting>().HasKey(x => x.Id);
            modelBuilder.Entity<FacultyManagementSuggestion>().HasKey(x => x.Id);
            modelBuilder.Entity<FacultyManagementSuggestionTutor>().HasKey(x => x.Id);
            
            modelBuilder.Entity<User>().Property(e => e.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => (GenderType)Enum.Parse(typeof(GenderType), v));  
            
            modelBuilder.Entity<User>().Property(e => e.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (RoleType)Enum.Parse(typeof(RoleType), v));
            
            modelBuilder.Entity<TutorSchedule>().Property(e => e.Meeting)
                .HasConversion(
                    v => v.ToString(),
                    v => (MeetingType)Enum.Parse(typeof(MeetingType), v));
            
            modelBuilder.Entity<TeacherAssistantSchedule>().Property(e => e.Meeting)
                .HasConversion(
                    v => v.ToString(),
                    v => (MeetingType)Enum.Parse(typeof(MeetingType), v));
            
            modelBuilder.Entity<Advertisement>().Property(e => e.Ticket)
                .HasConversion(
                    v => v.ToString(),
                    v => (TicketType)Enum.Parse(typeof(TicketType), v));

            modelBuilder.Entity<Apply>().Property(e => e.Ticket)
                .HasConversion(
                    v => v.ToString(),
                    v => (TicketType)Enum.Parse(typeof(TicketType), v));

            modelBuilder.Entity<TutorWeeklySchedule>().Property(e => e.Meeting)
                .HasConversion(
                    v => v.ToString(),
                    v => (MeetingType)Enum.Parse(typeof(MeetingType), v));
            
            modelBuilder.Entity<TutorWeeklySchedule>().Property(e => e.WeekDay)
                .HasConversion(
                    v => v.ToString(),
                    v => (WeekDayType)Enum.Parse(typeof(WeekDayType), v));

            modelBuilder.Entity<TeacherAssistantWeeklySchedule>().Property(e => e.Meeting)
                .HasConversion(
                    v => v.ToString(),
                    v => (MeetingType)Enum.Parse(typeof(MeetingType), v));
            
            modelBuilder.Entity<TeacherAssistantWeeklySchedule>().Property(e => e.WeekDay)
                .HasConversion(
                    v => v.ToString(),
                    v => (WeekDayType)Enum.Parse(typeof(WeekDayType), v));



        }
    }
}