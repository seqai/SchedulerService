using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IUpdateRepository<T>
    {
        TryOptionAsync<T> UpdateAsync(T entity);
    }
}
