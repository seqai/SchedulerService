namespace SchedulerService.DataAccess.Entities
{
    public class ScheduleEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public ICollection<CourseEntity> Courses { get; set; }
        public ICollection<StudyWeek> Weeks { get; set;  }
    }
}
