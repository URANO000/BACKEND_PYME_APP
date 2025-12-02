
using DataAccess.Models.DTOs.Login;
using DataAccess.Models.Entities;
using DataAccess.Repositories.Users;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, IUserRepository userRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        //login implementation
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            //Validate if it matches
            if(user == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return Unauthorized("Email o password incorrecto");
            }

            //Now we compare passwords using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            //Now validate the password
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return Unauthorized("Email o password incorrecto");
            }

            //If everything is ok, we generate the token
            var token = GenerateJwtToken(user);
            _logger.LogInformation($"Great, user was able to log in and toke {token} was generated");
            return Ok(new { token });
            
        }

        //Generate JWT token
        private string GenerateJwtToken(UsersDA user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName ?? string.Empty)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            //Return token
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
