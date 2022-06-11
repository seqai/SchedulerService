using Microsoft.AspNetCore.Mvc;
using SchedulerService.Business.Calculation;
using SchedulerService.Business.Models;
using SchedulerService.Controllers.InputModels;
using SchedulerService.DataAccess.Entities;
using SchedulerService.DataAccess.Repositories;
using SchedulerService.Infrastructure;
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
        public Task<ActionResult<StudySchedule>> Get(int id) => _readRepository.GetByIdAsync(id)
            .MatchActionResult(
                StudySchedule.FromEntity,
                NotFound, 
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Get)}: {ex.Message}");
                    return StatusCode(500);
                });

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<StudySchedule>), StatusCodes.Status200OK)]
        public Task<ActionResult<List<StudySchedule>>> Get(int skip, int take) => _readRepository.GetAsync(skip, take)
            .MatchActionResult(
                x => x.Select(StudySchedule.FromEntity).ToList(),
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Get)} (Many): {ex.Message}");
                    return StatusCode(500);
                });

        [HttpGet]
        [Route("calculation-strategies")]
        [ProducesResponseType(typeof(List<IdWithDescription<CalculationStrategy>>), StatusCodes.Status200OK)]
        public ActionResult<List<IdWithDescription<CalculationStrategy>>> Get() => Ok(
            Enum.GetValues(typeof(CalculationStrategy))
                .Cast<CalculationStrategy>()
                .Select(x => new IdWithDescription<CalculationStrategy>(x, x.GetDescription()))
                .ToList());

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(StudySchedule), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<StudySchedule>> Post([FromBody] ScheduleInputModel model) => 
            _studyScheduleService.CreateSchedule(
                model.StudentId,
                model.CourseIds,
                model.StartDate.Date,
                model.EndDate.Date,
                model.CalculationStrategy
            )
            .Match(
                either => either.MatchActionResult(
                    x =>  
                    {
                        _logger.LogInformation($"Created {nameof(StudySchedule)}: {x}");
                        return x;
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
        [ProducesResponseType(typeof(StudySchedule), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ActionResult<StudySchedule>> Preview([FromBody] ScheduleInputModel model) =>
            _studyScheduleService.CalculateSchedule(
                    model.StudentId,
                    model.CourseIds,
                    model.StartDate.Date,
                    model.EndDate.Date,
                    model.CalculationStrategy
                )
                .Match(
                    either => either.MatchActionResult(x => x, BadRequest),
                    ex =>
                    {
                        _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Post)}: {ex.Message}");
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
                    _logger.LogInformation($"{(result ? "Deleted" : "Attempted to delete non-existing")} {nameof(StudySchedule)} with id {id}");
                    return NoContent() as IActionResult;
                },
                ex =>
                {
                    _logger.LogError($"Error in {nameof(SchedulesController)}.{nameof(Delete)}: {ex.Message}");
                    return StatusCode(500);
                });
        
    }
}