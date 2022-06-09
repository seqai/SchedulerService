using SchedulerService.DataAccess.Entities;

namespace SchedulerService.Business.Models
{
    public record Course(int Id, string Name, int Length)
    {
        public CourseEntity ToEntity() => new()
        {
            Id = Id,
            Name = Name,
            Length = Length
        };

        public static Course FromEntity(CourseEntity entity) => new (entity.Id, entity.Name, entity.Length);
    }
}
