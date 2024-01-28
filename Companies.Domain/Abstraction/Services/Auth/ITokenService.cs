using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
