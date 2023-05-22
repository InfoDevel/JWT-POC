using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin login)
        {
            var user = Authenticate(login);
            if (user != null) {
                var token = Generate(user);
                return Ok(token);
            }
            return NotFound("User not Found");
        }

        private string Generate(User user)
        {
            var secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"]));
            var creds = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_configuration["JwtConfig:Issuer"],
                _configuration["JwtConfig:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        
        }

        private User Authenticate(UserLogin login) =>
            UsersConstants.Users.FirstOrDefault(x => x.UserName.ToLower() == login.UserName.ToLower() && x.Password == login.Password);

    }
}
