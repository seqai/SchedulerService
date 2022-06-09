namespace SchedulerService.DataAccess.Entities
{
    public class CourseEntity
    {
        private ICollection<StudyScheduleEntity>? _schedules;

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Length { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<StudyScheduleEntity> Schedules
        {
            get => _schedules ??= new List<StudyScheduleEntity>();
            set => _schedules = value;
        }
    }
}
