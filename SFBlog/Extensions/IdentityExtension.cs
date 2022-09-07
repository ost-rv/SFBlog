using System.Security.Claims;
using System.Security.Principal;

namespace SFBlog.Extensions
{
    public static class IdentityExtension
    {
        public static int GeUsertId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return 0;
            }

            return int.Parse(claim.Value);
        }
    }
}
