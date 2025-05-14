using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Server.DTOs;
using Project.Server.Models;
using Project.Server.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserService _userService;
        private readonly IEmailVerificationService _emailVerificationService;

        public AuthenticationController(IPasswordHasher passwordHasher, IUserService userService, IEmailVerificationService emailVerificationService)
        {
            this.passwordHasher = passwordHasher;
            _userService = userService;
            _emailVerificationService = emailVerificationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> CreateUser(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateUser(userDTO);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userDTO)
        {
            User user = _userService.GetUserByEmail(userDTO.Email);
            if (user != null && passwordHasher.VerifyHashedPassword(user.Password, userDTO.Password) == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(user.Email, user.Id);
                // Generate an authentication ticket (e.g., create a session or token for the user)
                var claims = new[] {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };
                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user (sets the authentication cookie)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Send CSRF token in the response as an HTTP-only cookie
                var antiforgery = HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
                antiforgery.SetCookieTokenAndHeader(HttpContext); // Set the CSRF token cookie

                return Ok(new
                {
                    Token = token,
                    User = _userService.GetUserDTO(user)
                });
            }

            return Unauthorized(new { Message = "Invalid Email or password!" });
        }

        private string GenerateJwtToken(string email, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, "User")
            };

            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                            ?? throw new InvalidOperationException("JWT_SECRET not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Jan Industries",
                audience: "People",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("check/email")]
        public IActionResult CheckEmailExists([FromQuery(Name = "email")] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email cannot be null or empty.");
            bool emailExists = _userService.CheckEmailExistsForAUser(email);
            return Ok(emailExists);
        }

        [HttpGet("check/username")]
        public IActionResult CheckUsernameExists([FromQuery(Name = "username")] string username) {
            if (string.IsNullOrEmpty(username)) return BadRequest("Username cannot be null or empty.");
            bool usernameExists = _userService.CheckUserNameExists(username);
            return Ok(usernameExists);
        }

        [HttpPost("check/password/email")]
        public IActionResult CheckPasswordAgainstEmail([FromBody] UserLoginDTO userDTO)
        {
            if (string.IsNullOrEmpty(userDTO.Password))
                return BadRequest("Password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(userDTO.Email))
                return BadRequest("Email cannot be null or empty.");

            var user = _userService.GetUserByEmail(userDTO.Email);
            if (user == null)
                return BadRequest("Password cannot be checked against a user that does not exist.");

            bool passwordMatches = _userService.CheckPasswordMatchesAgainstEmail(userDTO, user);
            return Ok(passwordMatches);
        }

        [Authorize]
        [HttpPost("update/user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updatedUser)
        {
            User user = _userService.GetLoggedInUser(User);
            await _userService.UpdateUser(user, updatedUser);

            if (updatedUser.Password != null) {
                await _userService.UpdateUserPassword(user, updatedUser.Password);
            }

            return Ok();
        }

        [HttpGet("csrf-token")]
        public IActionResult GetCsrfToken([FromServices] IAntiforgery antiforgery)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not logged in");
            }

            var csrfToken = Request.Cookies["XSRF-TOKEN"];

            if (string.IsNullOrEmpty(csrfToken))
            {
                return BadRequest("CSRF token not found in cookies");
            }

            return Ok(new { token = csrfToken });
        }

        [Authorize]
        [HttpPost("send/user-verification-code")]
        public async Task<IActionResult> SendUpdateEmailVerificationCode() {
            User user = _userService.GetLoggedInUser(User);
            await _emailVerificationService.SendEmailChangeCode(user);
            return Ok();
            
        }

        [Authorize]
        [HttpPost("verify/change-email")]
        public async Task<IActionResult> VerifyChangeEmailCode([FromQuery(Name ="code")] string code, [FromQuery(Name ="newEmail")] string newEmail) {
            User user = _userService.GetLoggedInUser(User);
            bool isValid = await _emailVerificationService.VerifyCode(user, code, newEmail);
            if (isValid) return Ok();
            else return Unauthorized();
        }




    }
}
