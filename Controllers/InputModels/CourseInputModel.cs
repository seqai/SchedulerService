using System.ComponentModel.DataAnnotations;

namespace SchedulerService.Controllers.InputModels
{
    public class CourseInputModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Length { get; set; }
    }
}
