using System.ComponentModel.DataAnnotations;

namespace TMS.DTOs
{
    public class ProjectDto
    {
        [Required(ErrorMessage = "Project Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Project Description is required")]
        public string Description { get; set; }

        //public ICollection<ProjectTaskDto>? Tasks { get; set; }
    }
}
