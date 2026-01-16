using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Domain.Entities;

namespace Application.Helper
{

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public CustomClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;


            var userIdClaim = new Claim(IdentityHelper.CustomClaimTypes.UserId, user.Id.ToString());
            identity.AddClaim(userIdClaim);
            // Add the user ID claim
            //identity.AddClaim(new Claim(IdentityHelper.CustomClaimTypes.UserId, user.Id.ToString()));

            return principal;
        }
    }

}
