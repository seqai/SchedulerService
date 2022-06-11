using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IFilterRepository<TFilter, T>
    {
        TryAsync<IReadOnlyCollection<T>> GetFilteredAsync(TFilter filter, int skip, int take);

    }
}
