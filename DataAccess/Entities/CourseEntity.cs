namespace SchedulerService.DataAccess.Entities
{
    public class CourseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Length { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ScheduleEntity> Schedules { get; set; }
    }
}
