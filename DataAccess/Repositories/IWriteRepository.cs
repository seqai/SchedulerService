﻿using LanguageExt;

namespace SchedulerService.DataAccess.Repositories
{
    public interface IWriteRepository<T>
    {
        TryAsync<T> CreateAsync(T entity);
        TryAsync<bool> DeleteByIdAsync(int id);
    }
}
