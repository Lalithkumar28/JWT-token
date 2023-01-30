using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JWT.BAL;
using JWT.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwtController : ControllerBase
    {
        public static LoginModel user=new LoginModel();
        private readonly IConfiguration _configure;
        private readonly IUserServices _context;

        public JwtController(IConfiguration configure,IUserServices context)
        {
            _configure = configure;
            _context = context;
        }
        
        [HttpGet]
        public ActionResult<string> GetClaim()
        {
            var use=_context.GetName();
            return Ok(use);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginModel>> Register(UserModel request)
        {
            CreatePassword(request.Password,out byte[] passwordHash, out byte[] passwordSalt);
            user.UserName=request.UserName;
            // user.Country="India";
            user.PasswordHash=passwordHash;
            user.passwordSalt=passwordSalt;
            return Ok(user);

        }
        [HttpPost("login")]

        public async Task<ActionResult<string>> Login(UserModel req)
        {
            if(user.UserName!=req.UserName)
            {
                return BadRequest("User not Found");
            }
            if(!VerifyPasswordHash(req.Password,user.PasswordHash,user.passwordSalt))
            {
                return Unauthorized("Enter Correct Password");
            }
            string token=CreateToken(user);
            return Ok(token);
            
        }

        private string CreateToken(LoginModel use)
        {
            List<Claim> claims=new List<Claim>
            {
                new Claim(ClaimTypes.Name,use.UserName),
                // new Claim(ClaimTypes.Country,use.Country),
                new Claim(ClaimTypes.Role,"Admin")
            };
            var key=new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configure.GetSection("AppSettings:Token").Value));
            var cred=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var Token=new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:cred);
            
            var jwt=new JwtSecurityTokenHandler().WriteToken(Token);

            return jwt;

        }

        private bool VerifyPasswordHash(string password,byte[] passwordHash,byte[] passwordSalt)
        {
            using(var hmac=new HMACSHA512(passwordSalt))
            {
                var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private void CreatePassword(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac=new HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}