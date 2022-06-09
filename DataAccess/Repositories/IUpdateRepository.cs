using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IUpdateRepository<T>
    {
        TryOptionAsync<int> UpdateAsync(T entity);
    }
}
