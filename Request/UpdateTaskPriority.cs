using TMS.HelperFunctions;

namespace TMS.Request
{
    public class UpdateTaskPriority
    {
        public Guid TaskId { get; set; }
        [Priority]
        public string Priority { get; set; }   
    }
}
