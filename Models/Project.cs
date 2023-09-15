namespace TMS.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }  
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProjectTask>? Tasks { get; set; }    
    }
}
