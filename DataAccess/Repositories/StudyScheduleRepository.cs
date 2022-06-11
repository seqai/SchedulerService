using LanguageExt;
using Microsoft.EntityFrameworkCore;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Filters;
using static LanguageExt.Prelude;
using Array = System.Array;

namespace SchedulerService.DataAccess.Repositories
{
    internal class StudyScheduleRepository : IReadRepository<StudyScheduleEntity>, IFilterRepository<StudyScheduleFilter, StudyScheduleEntity>, IWriteRepository<StudyScheduleEntity>
    {
        private readonly SchedulerServiceDbContext _context;

        public StudyScheduleRepository(SchedulerServiceDbContext context)
        {
            _context = context;
        }

        public TryOptionAsync<StudyScheduleEntity> GetByIdAsync(int id) 
            => TryOptionAsync<StudyScheduleEntity>(_context.Schedules
                .Include(x => x.Courses)
                .Include(x => x.Weeks)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id)!);

        public TryAsync<IReadOnlyCollection<StudyScheduleEntity>> GetByIdsAsync(IReadOnlyCollection<int> ids) 
            => TryAsync(_context.Schedules.Where(x => ids.Contains(x.Id)).ToList() as IReadOnlyCollection<StudyScheduleEntity>);

        public TryAsync<IReadOnlyCollection<StudyScheduleEntity>> GetAsync(int skip, int take) =>
            GetFilteredAsync(new StudyScheduleFilter(Array.Empty<int>()), skip, take);

        public TryAsync<IReadOnlyCollection<StudyScheduleEntity>> GetFilteredAsync(StudyScheduleFilter filter, int skip, int take)
        {
            var courses = _context.Schedules
                .Include(x => x.Courses)
                .Include(x => x.Weeks)
                .Where(x => !x.IsDeleted);

            if (filter.studentIds.Count > 0)
            {
                courses = courses.Where(c => filter.studentIds.Contains(c.StudentId));
            }
            if (skip > 0)
            {
                courses = courses.Skip(skip);
            }
            if (take > 0)
            {
                courses = courses.Take(take);
            }
            
            return TryAsync(courses.ToListAsync()).Map(x => x.AsReadOnly() as IReadOnlyCollection<StudyScheduleEntity>);
        }

        public TryAsync<int> CreateAsync(StudyScheduleEntity entity) => async () =>
        {
            foreach (var course in entity.Courses)
            {
                _context.Attach(course);
                await _context.Entry(course).ReloadAsync();
            }
            var created = _context.Add(entity);
            await _context.SaveChangesAsync();
            return created.Entity.Id;
        };

        public TryAsync<bool> DeleteByIdAsync(int id) => GetByIdAsync(id)
            .MapAsync(async course =>
            {
                course.IsDeleted = true; 
                await _context.SaveChangesAsync();
                return true;
            }).ToTry(() => false);
    }
}
