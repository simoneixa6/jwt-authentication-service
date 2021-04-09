using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwt_authentication_service.Models
{
    public class AuthenticatedUser
    {
        public string Id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        
        public AuthenticatedUser(User user, String token)
        {
            this.Id = user.Id;
            this.username = user.username;
            this.token = token;
        }
    }
}
