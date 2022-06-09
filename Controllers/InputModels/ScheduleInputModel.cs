using System.ComponentModel.DataAnnotations;
using SchedulerService.Business.Calculation;

namespace SchedulerService.Controllers.InputModels
{
    public class ScheduleInputModel : IValidatableObject
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [MinLength(1)]
        public IReadOnlyCollection<int> CourseIds { get; set; } = Array.Empty<int>();
        [Required]
        public CalculationStrategy CalculationStrategy { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.Date < StartDate.Date)
            {
                yield return new ValidationResult($"{nameof(EndDate)} must be greater or equal than {nameof(StartDate)}", new []{ nameof(StartDate), nameof(EndDate)});
            }
        }
    }
}
