namespace TMS.Models
{
    public class Email
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public string template { get; set; }    
        public string userName { get; set; }    
        public string userEmail { get; set; }
        public string message { get; set; }
        public string subject { get; set; }
        public string type { get; set; }    
        public bool isEmailSent { get; set; } = false;
        public string createdOn { get; set; } = DateTime.Now.ToString("dd/MMM/yyyy : HH:mm");
        public int retries { get; set; } = 0;
    }
}
