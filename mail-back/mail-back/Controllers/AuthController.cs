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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IOptions<AuthOptions> authOptions;
        UserRepository db;
        Logger logger = LogManager.GetCurrentClassLogger();

        public AuthController(IOptions<AuthOptions> authOptions, IOptionsSnapshot<DBConfig> dbConfig)
        {
            this.authOptions = authOptions;
            string connectionString = dbConfig.Value.SqliteConnection;
            db = new UserRepository(connectionString);
        }

        [HttpPost]
        [Route("sign-up")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                db.InsertUser(user);
                return Ok();
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }


        [HttpPost]
        [Route("sign-in")]
        public IActionResult Login([FromBody] User userReq)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex);
                return Unauthorized();
            }
        }

        private User AuthUser(string username, string password)
        {
            try
            {
                return db.GetUsers().SingleOrDefault(u => u.Username == username && u.Password == GetHashPassword(password));
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }
        private string GenerateJWTToken(User user)
        {
            try
            {
                var authParam = authOptions.Value;
                var securityKey = authParam.GetSymmetricSecurityKey();
                var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.IdRole.ToString())
                };
                var token = new JwtSecurityToken(authParam.Issuer, authParam.Audience, claims,
                    expires: DateTime.Now.AddSeconds(authParam.TokenLifetime), signingCredentials: credentails);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
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
