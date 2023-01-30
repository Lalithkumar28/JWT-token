using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT.Model
{
    public class LoginModel
    {
        public string? UserName { get; set; }
 
        public string? Country{get; set;}
        public byte[]? PasswordHash { get; set; } 

        public byte[]? passwordSalt {get; set;}
    }
}