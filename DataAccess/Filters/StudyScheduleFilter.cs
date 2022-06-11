namespace SchedulerService.DataAccess.Filters
{
    public record StudyScheduleFilter(IReadOnlyCollection<int> studentIds);
}
