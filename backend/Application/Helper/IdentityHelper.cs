using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public static class IdentityHelper
    {
        public static long GetUserId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.UserId);

            if (claim == null)
                return 0; //new Guid();

            return Convert.ToInt64(claim.Value);    //Guid.Parse(claim.Value);
        }
        public static class CustomClaimTypes
        {
            public const string UserId = "UserId";
            public const string UserName = "UserName";
            //public const string UserTypeId = "Role";
            //public const string NGO = "NGOId";
            //public const string UserImageFilePath = "UserImageFilePath";
            //public const string IsSubordinate = "IsSubordinate";
            //public const string ParentId = "ParentId";

        }
    }
}
