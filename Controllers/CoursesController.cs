using Microsoft.AspNetCore.Mvc;
using SchedulerService.Business.Models;
using SchedulerService.Controllers.InputModels;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;
using SchedulerService.Infrastructure;

namespace SchedulerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IReadRepository<CourseEntity> _readRepository;
        private readonly IWriteRepository<CourseEntity> _writeRepository;
        private readonly IUpdateRepository<CourseEntity> _updateRepository;

        public CoursesController(
            ILogger<CoursesController> logger,
            IReadRepository<CourseEntity> readRepository,
            IWriteRepository<CourseEntity> writeRepository,
            IUpdateRepository<CourseEntity> updateRepository)
        {
            _logger = logger;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _updateRepository = updateRepository;
        }

        [HttpGet]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<Course>> Get(int id) => _readRepository.GetByIdAsync(id)
            .MatchActionResult(
                Course.FromEntity, 
                NotFound, 
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Get)}: {ex.Message}");
                    return StatusCode(500);
                });

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Course>), StatusCodes.Status200OK)]
        public Task<ActionResult<List<Course>>> Get(int skip, int take) => _readRepository.GetAsync(skip, take)
            .MatchActionResult(
                x => x.Select(Course.FromEntity).ToList(),
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Get)} (Many): {ex.Message}");
                    return StatusCode(500);
                });


        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<Course>> Post([FromBody] CourseInputModel model) => 
            _writeRepository.CreateAsync(new CourseEntity() { Name = model.Name, Length = model.Length })
                .Bind(id => _readRepository.GetByIdAsync(id).ToTry(() => throw new Exception($"{nameof(Course)} was created by the repository but could not be retrieved by id: {id}")))
                .MatchActionResult(
                x =>
                {
                    _logger.LogInformation($"Created {nameof(Course)}: {Course.FromEntity(x)}");
                    return Course.FromEntity(x);
                },
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Post)}: {ex.Message}");
                    return StatusCode(500);
                });

        [HttpPut]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<Course>> Put([FromRoute] int id, [FromBody] CourseInputModel model) =>
            // NB: check schedules and return 409 if length is changed for used schedule or invalidate/change saved schedule
            _updateRepository.UpdateAsync(new CourseEntity() { Id = id, Name = model.Name, Length = model.Length })
                .Bind(id => _readRepository.GetByIdAsync(id))
                .MatchActionResult(
                    x =>
                    {
                        _logger.LogInformation($"Updated {nameof(Course)}: {Course.FromEntity(x)}");
                        return Course.FromEntity(x);
                    },
                    NotFound,
                    ex =>
                    {
                        _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Put)}: {ex.Message}");
                        return StatusCode(500);
                    });

        [HttpDelete]
        [Route("{id:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete([FromRoute] int id) => _writeRepository.DeleteByIdAsync(id)
            .Match(
                result =>
                {
                    _logger.LogInformation($"{(result ? "Deleted" : "Attempted to delete non-existing")} {nameof(Course)} with id {id}");
                    return NoContent();
                },
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Delete)}: {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });
        
    }
}