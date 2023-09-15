using System.ComponentModel.DataAnnotations;
using TMS.HelperFunctions;

namespace TMS.DTOs
{
    public class ProjectTaskDto
    {
        [Required(ErrorMessage ="Project Id is required")]
        public Guid ProjectId { get; set; } 
        [Required (ErrorMessage ="Task Title is required")]
        public string Title { get; set; }

        [Required (ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required (ErrorMessage = "Due date is required")]
        [ValidDate]
        public string DueDate { get; set; }

        [Required (ErrorMessage = "Priority is required")]
        public string Priority { get; set; }

        [Required (ErrorMessage = "Status is required")]
        public string Status { get; set; }
    }
}
