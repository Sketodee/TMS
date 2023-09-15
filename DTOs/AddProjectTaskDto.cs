using System.ComponentModel.DataAnnotations;
using TMS.HelperFunctions;

namespace TMS.DTOs
{
    public class AddProjectTaskDto
    {
        [Required(ErrorMessage = "Project Id is required")]
        public Guid ProjectId { get; set; }
        [Required(ErrorMessage = "Task Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [ValidDate]
        public string DueDate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Priority]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Status]
        public string Status { get; set; }
    }
}
