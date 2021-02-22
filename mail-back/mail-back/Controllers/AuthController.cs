using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using mail_back.Models;
using mail_back.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IOptions<AuthOptions> authOptions;
        UserRepository db;
        public AuthController(IOptions<AuthOptions> authOptions, IConfiguration configuration)
        {
            this.authOptions = authOptions;
            string connectionString = configuration.GetConnectionString("sqlite");
            db = new UserRepository(connectionString);
        }

        [HttpPost]
        [Route("sign-up")]
        public IActionResult Register([FromBody] User user)
        {
            db.InsertUser(user);
            return Ok();
        }


        [HttpPost]
        [Route("sign-in")]
        public IActionResult Login([FromBody] User userReq)
        {
            var user = AuthUser(userReq.Username, userReq.Password);
            if (user != null)
            {
                var token = GenerateJWTToken(user);
                return Ok(new
                {
                    access_token = token,
                    role = user.IdRole
                });
            }

            return Unauthorized();
        }

        private User AuthUser(string username, string password)
        {
            return db.GetUsers().SingleOrDefault(u => u.Username == username && u.Password == GetHashPassword(password));
        }
        private string GenerateJWTToken(User user)
        {
            var authParam = authOptions.Value;
            var securityKey = authParam.GetSymmetricSecurityKey();
            var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };
            var token = new JwtSecurityToken(authParam.Issuer, authParam.Audience, claims,
                expires: DateTime.Now.AddSeconds(authParam.TokenLifetime), signingCredentials: credentails);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GetHashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes);
                return hash;
            }
        }
    }
}
