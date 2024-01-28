using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services.Auth;
using Companies.Infrastructure.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
                _userRepository= userRepository;
        }
        public async Task<User> Get(UserLogin userLogin)
        {
            return await _userRepository.GetUserByUsername(userLogin.Username);
        }
        public async Task AddUser(Register newUser)
        {
            var existingUser = await _userRepository.GetUserByUsername(newUser.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            await _userRepository.AddUser(newUser);
        }
    }
}
