using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IReadRepository<T>
    {
        TryOptionAsync<T> GetByIdAsync(int id);
        TryAsync<IReadOnlyCollection<T>> GetAsync(int skip, int take);

    }
}
