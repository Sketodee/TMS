namespace TMS.Request
{
    public class UpdateTask
    {
        public Guid TaskId { get; set; }
        public string StatusOrPriority { get; set; }   
    }
}
