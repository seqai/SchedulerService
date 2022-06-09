using SchedulerService.DataAccess.Entities;

namespace SchedulerService.Business.Models
{
    public record Course(int Id, string Name, int Length)
    {
        public static Course FromEntity(CourseEntity entity) => new Course(entity.Id, entity.Name, entity.Length);
    }
}
