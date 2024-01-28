using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Infrastructure.Models.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
    }
}
