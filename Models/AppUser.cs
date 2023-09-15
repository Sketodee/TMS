using Microsoft.AspNetCore.Identity;

namespace TMS.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }    
    }
}
