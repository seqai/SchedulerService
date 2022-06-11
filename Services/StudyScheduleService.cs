using LanguageExt;
using SchedulerService.Business.Calculation;
using SchedulerService.Business.Models;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;
using static LanguageExt.Prelude;
using static SchedulerService.Business.Calculation.ScheduleCalculator;

namespace SchedulerService.Services
{
    public class StudyScheduleService
    {
        private readonly IWriteRepository<StudyScheduleEntity> _scheduleWriteRepository;
        private readonly IReadRepository<CourseEntity> _courseReadRepository;

        public StudyScheduleService(IWriteRepository<StudyScheduleEntity> scheduleWriteRepository, IReadRepository<CourseEntity> courseReadRepository)
        {
            _scheduleWriteRepository = scheduleWriteRepository;
            _courseReadRepository = courseReadRepository;
        }

        public TryAsync<Either<string, StudySchedule>> CalculateSchedule(int studentId, IReadOnlyCollection<int> courseIds, DateTime startDate, DateTime endDate, CalculationStrategy strategy) =>
            from coursesResult in GetCourses(courseIds) 
            select from courses in coursesResult  
                   from weeklyHours in CalculateWeeklyStudyHours(startDate, endDate, courses.Select(x => x.Length).ToList(), strategy)
                   select new StudySchedule(0, studentId, startDate, endDate, courses.Select(Course.FromEntity).ToList(), weeklyHours);

        public TryAsync<Either<string, StudySchedule>> CreateSchedule(int studentId, IReadOnlyCollection<int> courseIds, DateTime startDate, DateTime endDate, CalculationStrategy strategy) => 
            from calculationResult in CalculateSchedule(studentId, courseIds, startDate, endDate, strategy)
            from writeResult in calculationResult.Map(schedule => _scheduleWriteRepository.CreateAsync(schedule.ToEntity())).Sequence()
            select from schedule in calculationResult 
                   from id in writeResult select schedule with { Id = id };

        private TryAsync<Either<string, IReadOnlyCollection<CourseEntity>>> GetCourses(IReadOnlyCollection<int> courseIds)
        {
            string GetMissingIds(IEnumerable<CourseEntity> courses)
            {
                var existing = courses.Select(x => x.Id).ToHashSet();
                return string.Join(", ", courseIds.Where(x => !existing.Contains(x)));
            }

            return _courseReadRepository.GetByIdsAsync(courseIds).Map(cs => ((cs.Count == courseIds.Count)
                ? Right<string, IReadOnlyCollection<CourseEntity>>(cs)
                : Left($"Some of the course ids are missing: {GetMissingIds(cs)}!")));
        }
    }
}
