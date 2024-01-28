using Companies.Infrastructure.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services.Auth
{
    public interface IUserService
    {
        Task<User> Get(UserLogin userLogin);
        Task AddUser(User newUser);
    }
}
