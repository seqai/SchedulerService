using Microsoft.AspNetCore.Mvc;
using SchedulerService.Business.Models;
using SchedulerService.Controllers.InputModels;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;

namespace SchedulerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IReadRepository<CourseEntity> _readRepository;
        private readonly IWriteRepository<CourseEntity> _writeRepository;

        public CoursesController(
            ILogger<CoursesController> logger,
            IReadRepository<CourseEntity> readRepository,
            IWriteRepository<CourseEntity> writeRepository)
        {
            _logger = logger;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        [HttpGet]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get(int id) => _readRepository.GetByIdAsync(id)
            .Match(
                x => Ok(Course.FromEntity(x)), 
                NotFound, 
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Get)}: {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Course>), StatusCodes.Status200OK)]
        public Task<IActionResult> Get(int skip, int take) => _readRepository.GetAsync(skip, take)
            .Match(
                x => Ok(x.Select(Course.FromEntity).ToList()),
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Get)} (Many): {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });


        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(List<Course>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Post([FromBody] CourseInputModel model) => 
            _writeRepository.CreateAsync(new CourseEntity() { Name = model.Name, Length = model.Length })
            .Match(
                x =>
                {
                    _logger.LogInformation($"Created {nameof(Course)}: {Course.FromEntity(x)}");
                    return Ok(Course.FromEntity(x));
                },
                ex =>
                {
                    _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Post)}: {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });

        [HttpPut]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Put([FromRoute] int id, [FromBody] CourseInputModel model) =>
            _writeRepository.UpdateAsync(new CourseEntity() { Id = id, Name = model.Name, Length = model.Length })
                .Match(
                    x =>
                    {
                        _logger.LogInformation($"Updated {nameof(Course)}: {Course.FromEntity(x)}");
                        return Ok(Course.FromEntity(x));
                    },
                    NotFound,
                    ex =>
                    {
                        _logger.LogError($"Error in {nameof(CoursesController)}.{nameof(Put)}: {ex.Message}");
                        return StatusCode(500) as IActionResult;
                    });

        [HttpDelete]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(List<Course>), StatusCodes.Status204NoContent)]
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