using Companies.Domain.Abstraction.Services.Auth;
using Companies.Infrastructure.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Auth
{
    public class ClaimService:IClaimService
    {
        public IEnumerable<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Role, user.Role)
            };
        }
    }
}
