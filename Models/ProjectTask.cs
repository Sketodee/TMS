using System.ComponentModel.DataAnnotations;

namespace TMS.Models
{
    public class ProjectTask
    {
        public Guid Id { get; set; }  = Guid.NewGuid();

        public Project? Project { get; set; }

        public Guid ProjectId { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }    

        public string Description { get; set; }

        public string DueDate { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }  
    }
}
