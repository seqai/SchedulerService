using LanguageExt;
using Microsoft.EntityFrameworkCore;
using SchedulerService.DataAccess.Entities;
using static LanguageExt.Prelude;

namespace SchedulerService.DataAccess.Repositories
{
    internal class CourseRepository : IReadRepository<CourseEntity>, IWriteRepository<CourseEntity>, IUpdateRepository<CourseEntity>
    {
        private readonly SchedulerServiceDbContext _context;

        public CourseRepository(SchedulerServiceDbContext context)
        {
            _context = context;
        }

        public TryOptionAsync<CourseEntity> GetByIdAsync(int id) 
            => TryOptionAsync<CourseEntity>(_context.Courses.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id)!);

        public TryAsync<IReadOnlyCollection<CourseEntity>> GetByIdsAsync(IReadOnlyCollection<int> ids) 
            => TryAsync(_context.Courses.Where(x => ids.Contains(x.Id)).ToList() as IReadOnlyCollection<CourseEntity>);

        public TryAsync<IReadOnlyCollection<CourseEntity>> GetAsync(int skip, int take)
        {
            var courses = _context.Courses.Where(x => !x.IsDeleted);
            if (skip > 0)
            {
                courses = courses.Skip(skip);
            }
            if (take > 0)
            {
                courses = courses.Take(take);
            }
            
            return TryAsync(courses.ToListAsync()).Map(x => x.AsReadOnly() as IReadOnlyCollection<CourseEntity>);
        }

        public TryAsync<int> CreateAsync(CourseEntity entity) => async () =>
        {
            var created = _context.Add(entity);
            await _context.SaveChangesAsync();
            return created.Entity.Id;
        };

        public TryOptionAsync<int> UpdateAsync(CourseEntity entity) => GetByIdAsync(entity.Id)
            .MapAsync(async course => {
                course.Name = entity.Name;
                course.Length = entity.Length;
                await _context.SaveChangesAsync();
                return course.Id;
            });

        public TryAsync<bool> DeleteByIdAsync(int id) => GetByIdAsync(id)
            .MapAsync(async course =>
            {
                course.IsDeleted = true; 
                await _context.SaveChangesAsync();
                return true;
            }).ToTry(() => false);
    }
}
