using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace TMS.HelperFunctions
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        public CustomAuthorize(params AuthorizeRoles[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
