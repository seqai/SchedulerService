using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IWriteRepository<T>
    {
        TryAsync<int> CreateAsync(T entity);
        TryAsync<bool> DeleteByIdAsync(int id);
    }
}
