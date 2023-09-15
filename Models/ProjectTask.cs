using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TMS.Models
{
    public class ProjectTask
    {
        public Guid Id { get; set; }  = Guid.NewGuid();
        [JsonIgnore]
        [IgnoreDataMember]
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
