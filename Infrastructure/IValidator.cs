namespace SchedulerService.Infrastructure
{
    internal interface IValidator<in T>
    {
        IEnumerable<string> Validate(T instance);
    }
}
