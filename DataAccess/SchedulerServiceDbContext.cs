﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchedulerService.DataAccess.Entities;
using SchedulerService.Infrastructure.Configuration;

namespace SchedulerService.DataAccess
{
    internal class SchedulerServiceDbContext : DbContext
    {
        public DbSet<CourseEntity> Courses { get; set; } = null!;
        public DbSet<ScheduleEntity> Schedules { get; set; } = null!;

        private readonly string _connectionString;

        public SchedulerServiceDbContext(IOptions<PersistenceConfiguration> configuration)
        {
            _connectionString = configuration.Value.ConnectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<CourseEntity>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<CourseEntity>().Property(x => x.Length).IsRequired();

            modelBuilder.Entity<ScheduleEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleEntity>().HasIndex(x => x.StudentId);
            modelBuilder.Entity<ScheduleEntity>().HasMany(x => x.Courses).WithMany(y => y.Schedules).UsingEntity(z => z.ToTable("ScheduleCourses"));
            modelBuilder.Entity<ScheduleEntity>().HasMany(x => x.Weeks).WithOne().HasForeignKey(x => x.ScheduleId);

            modelBuilder.Entity<StudyWeek>().HasKey(x => new { x.ScheduleId, x.WeekNumber });
        }
    }
}
