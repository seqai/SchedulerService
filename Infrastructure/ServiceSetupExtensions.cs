using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchedulerService.DataAccess;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;
using SchedulerService.Infrastructure.Configuration;

namespace SchedulerService.Infrastructure
{
    internal static class ServiceSetupExtensions
    {
        public static void RegisterSchedulerServiceDependencies(this WebApplicationBuilder builder)
        {
            var persistenceConfiguration = builder.Configuration.GetSection("Persistence");
            builder.Services.Configure<PersistenceConfiguration>(persistenceConfiguration);
            builder.Services.AddTransient<SchedulerServiceDbContext>();
            builder.Services.AddScoped<CourseRepository>();
            builder.Services.AddScoped<IWriteRepository<CourseEntity>>(x => x.GetRequiredService<CourseRepository>());
            builder.Services.AddScoped<IReadRepository<CourseEntity>>(x => x.GetRequiredService<CourseRepository>());

        }

        public static void UpdateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<SchedulerServiceDbContext>();
            var configuration = serviceScope.ServiceProvider.GetService<IOptions<PersistenceConfiguration>>()?.Value ?? new PersistenceConfiguration();

            if (configuration.AutomaticMigration)
            {
                context!.Database.Migrate();
            }

            if (configuration.SeedSampleCourses)
            {
                using (context)
                {
                    if (context.Courses.Count() > 0)
                    {
                        return;
                    }
                    context.Courses.AddRange(
                        new CourseEntity { Name = "Blockchain and HR", Length = 8 },
                        new CourseEntity { Name = "Compensation & Benefits", Length = 32 },
                        new CourseEntity { Name = "Digital HR", Length = 40 },
                        new CourseEntity { Name = "Digital HR Strategy", Length = 10 },
                        new CourseEntity { Name = "Digital HR Transformation", Length = 8 },
                        new CourseEntity { Name = "Diversity & Inclusion", Length = 20 },
                        new CourseEntity { Name = "Employee Experience & Design Thinking", Length = 12 },
                        new CourseEntity { Name = "Employer Branding", Length = 6 },
                        new CourseEntity { Name = "Global Data Integrity", Length = 12 },
                        new CourseEntity { Name = "Hiring & Recruitment Strategy", Length = 15 },
                        new CourseEntity { Name = "HR Analytics Leader", Length = 21 },
                        new CourseEntity { Name = "HR Business Partner 2.0", Length = 40 },
                        new CourseEntity { Name = "HR Data Analyst", Length = 18 },
                        new CourseEntity { Name = "HR Data Science in R", Length = 12 },
                        new CourseEntity { Name = "HR Data Visualization", Length = 12 },
                        new CourseEntity { Name = "HR Metrics & Reporting", Length = 40 },
                        new CourseEntity { Name = "Learning & Development", Length = 30 },
                        new CourseEntity { Name = "Organizational Development", Length = 30 },
                        new CourseEntity { Name = "People Analytics", Length = 40 },
                        new CourseEntity { Name = "Statistics in HR", Length = 15 },
                        new CourseEntity { Name = "Strategic HR Leadership", Length = 34 },
                        new CourseEntity { Name = "Strategic HR Metrics", Length = 17 },
                        new CourseEntity { Name = "Talent Acquisition", Length = 40 });
                    context.SaveChanges();
                }
            }
        }
    }
}
