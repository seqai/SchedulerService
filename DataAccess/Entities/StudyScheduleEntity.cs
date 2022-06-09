namespace SchedulerService.DataAccess.Entities
{
    public class StudyScheduleEntity
    {
        private ICollection<CourseEntity>? _courses;
        private ICollection<StudyWeekEntity>? _weeks;

        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<CourseEntity> Courses
        {
            get => _courses ??= new List<CourseEntity>();
            set => _courses = value;
        }

        public ICollection<StudyWeekEntity> Weeks
        {
            get => _weeks ??= new List<StudyWeekEntity>();
            set => _weeks = value;
        }

        public bool IsDeleted { get; set; }
    }
}
