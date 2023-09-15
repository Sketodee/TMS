using TMS.HelperFunctions;

namespace TMS.Request
{
    public class UpdateTaskStatus
    {
        public Guid TaskId { get; set; }
        [Status]
        public string Status { get; set; }   
    }
}
