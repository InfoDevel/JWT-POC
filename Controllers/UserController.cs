using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWT_POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("public endpoint");
        }

        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminsEndpoint()
        {
            var currentUser = GetCurrentuser();
            return Ok($"User {currentUser?.UserName}, Role {currentUser?.Role}");
        }

        private User? GetCurrentuser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    UserName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }


        [HttpPost("NewUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateNewUser([FromBody] User user)
        {
            UsersConstants.Users.Add(user);
            return Ok("new user created! try login in with his credentials and then accessing some authorized endpoints using his jwt.");
        }

        [HttpGet("ChangeRole")]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUserRole([FromBody] NewRole role)
        {
            if (!UsersConstants.Users.Any(x => x.UserName == role.UserName) || !UsersConstants.Roles.Any(x => x == role.Role))
            {
                return NotFound("user or role not found");
            }
            var user = UsersConstants.Users.FirstOrDefault(x => x.UserName == role.UserName);
            user.Role = role.Role;
            return Ok("new user created! try login in with his credentials and then accessing some authorized endpoints using his jwt.");
        }

    }
}
