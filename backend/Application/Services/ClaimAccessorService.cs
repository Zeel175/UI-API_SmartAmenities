using Application.Helper;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClaimAccessorService : IClaimAccessorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClaimAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetUserId()
        {
            var toReturn = GetTokenClaim("UserId");
            return long.Parse(toReturn);
        }
        //public int? UserType()
        //{
        //    var toReturn = GetTokenClaim("userType");
        //    return toReturn?.ToInteger();
        //}

        private string GetTokenClaim(string type)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string authHeader = httpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var claim = jwtToken?.Claims.FirstOrDefault(c => c.Type == type);
                return claim?.Value;
            }

            return null;

        }
    }
}
