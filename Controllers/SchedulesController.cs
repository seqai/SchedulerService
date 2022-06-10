using Microsoft.AspNetCore.Mvc;
using SchedulerService.Business.Models;
using SchedulerService.Controllers.InputModels;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;
using SchedulerService.Services;

namespace SchedulerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly ILogger<SchedulesController> _logger;
        private readonly IReadRepository<StudyScheduleEntity> _readRepository;
        private readonly IWriteRepository<StudyScheduleEntity> _writeRepository;
        private readonly StudyScheduleService _studyScheduleService;

        public SchedulesController(
            ILogger<SchedulesController> logger,
            IReadRepository<StudyScheduleEntity> readRepository, 
            IWriteRepository<StudyScheduleEntity> writeRepository,
            StudyScheduleService studyScheduleService)
        {
            _logger = logger;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _studyScheduleService = studyScheduleService;
        }

        [HttpGet]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(StudySchedule), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get(int id) => _readRepository.GetByIdAsync(id)
            .Match(
                x => Ok(StudySchedule.FromEntity(x)), 
                NotFound, 
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Get)}: {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<StudySchedule>), StatusCodes.Status200OK)]
        public Task<IActionResult> Get(int skip, int take) => _readRepository.GetAsync(skip, take)
            .Match(
                x => Ok(x.Select(StudySchedule.FromEntity).ToList()),
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Get)} (Many): {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });


        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(List<StudySchedule>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Post([FromBody] ScheduleInputModel model) => 
            _studyScheduleService.CreateSchedule(
                model.StudentId,
                model.CourseIds,
                model.StartDate.Date,
                model.EndDate.Date,
                model.CalculationStrategy
            )
            .Match(
                either => either.Match(
                    x =>  
                    {
                        _logger.LogInformation($"Created {nameof(StudySchedule)}: {x}");
                        return Ok(x) as IActionResult;
                    },
                    BadRequest
                ),
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Post)}: {ex.Message}");
                    return StatusCode(500);
                });


        [HttpPost]
        [Route("preview")]
        [ProducesResponseType(typeof(List<StudySchedule>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Preview([FromBody] ScheduleInputModel model) =>
            _studyScheduleService.CalculateSchedule(
                    model.StudentId,
                    model.CourseIds,
                    model.StartDate.Date,
                    model.EndDate.Date,
                    model.CalculationStrategy
                )
                .Match(
                    either => either.Match(
                        x => Ok(x) as IActionResult,
                        BadRequest
                    ),
                    ex =>
                    {
                        _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Post)}: {ex.Message}");
                        return StatusCode(500);
                    });

        [HttpDelete]
        [Route("{id:min(1)}")]
        [ProducesResponseType(typeof(List<StudySchedule>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete([FromRoute] int id) => _writeRepository.DeleteByIdAsync(id)
            .Match(
                result =>
                {
                    _logger.LogInformation($"{(result ? "Deleted" : "Attempted to delete non-existing")} {nameof(StudySchedule)} with id {id}");
                    return NoContent();
                },
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Delete)}: {ex.Message}");
                    return StatusCode(500) as IActionResult;
                });
        
    }
}