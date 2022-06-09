using SchedulerService.DataAccess.Entities;

namespace SchedulerService.Business.Models
{
    public record StudySchedule(int Id, int StudentId, DateTime StartDate, DateTime EndDate, IReadOnlyCollection<Course> Courses, IReadOnlyList<int> WeeklyHours)
    {
        public StudyScheduleEntity ToEntity() => new()
        {
            Id = Id,
            StudentId = StudentId,
            StartDate = StartDate.Date,
            EndDate = EndDate.Date,
            Courses = Courses.Select(x => x.ToEntity()).ToList(),
            Weeks = WeeklyHours.Select((x, i) => new StudyWeekEntity() { WeekNumber = i + 1, StudyHours = x }).ToList()
        };

        public static StudySchedule FromEntity(StudyScheduleEntity entity) => new(
            entity.Id,
            entity.StudentId,
            entity.StartDate,
            entity.EndDate,
            entity.Courses.Select(Course.FromEntity).ToList().AsReadOnly(),
            entity.Weeks.OrderBy(x => x.WeekNumber).Select(x => x.StudyHours).ToList().AsReadOnly());
    }
}
