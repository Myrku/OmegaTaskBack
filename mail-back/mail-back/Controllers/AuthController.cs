using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using mail_back.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IOptions<AuthOptions> authOptions;
        public AuthController(IOptions<AuthOptions> authoptions)
        {
            authOptions = authoptions;
        }
        List<User> users = new List<User>()
        {
            new User() {id = 1, username = "test22", email="test@maul.ru", password="qwerty1234", role="admin"}
        };

        [HttpPost]
        [Route("sign-in")]
        public IActionResult Login([FromBody] User user_req)
        {
            var user = AuthUser(user_req.username, user_req.password);
            if (user != null)
            {
                var token = GenerateJWTToken(user);
                return Ok(new
                {
                    access_token = token
                });
            }

            return Unauthorized();
        }

        private User AuthUser(string username, string password)
        {
            return users.SingleOrDefault(u => u.username == username && u.password == password);
        }
        private string GenerateJWTToken(User user)
        {
            var authParam = authOptions.Value;
            var securityKey = authParam.GetSymmetricSecurityKey();
            var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.username),
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
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
