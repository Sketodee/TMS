using TMS.HelperFunctions;

namespace TMS.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public string UserId { get; set; }  
        public string Title { get; set; }   
        public string Description { get; set; }
        public string Status { get; set; } = NotificationStatus.Unread.ToString();
        public string CreatedOn { get; set; } = DateTime.Now.ToString("dd/MMM/yyyy");
    }
}
