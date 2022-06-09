namespace SchedulerService.Infrastructure.Configuration
{
    public class PersistenceConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public bool AutomaticMigration { get; set; }
        public bool SeedSampleCourses { get; set; }
    }
}
