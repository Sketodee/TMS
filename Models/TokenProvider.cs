using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Models
{
    public class TokenProvider
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime SignedUp { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; } = DateTime.Now.AddMinutes(1);
    }
}
