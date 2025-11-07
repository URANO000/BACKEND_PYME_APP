using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class UsersController : ControllerBase
    {
        //Call my service
        private readonly IUsersService _usersService;
        //call logger
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        //Now I can implement my HTTP METHODS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersDTO>>> GetAllUsers()
        {
            try
            {
                var users = await _usersService.GetAllUsersAsync();
                _logger.LogInformation("Users retrieved successfully.");
                return Ok(users);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersDTO>> GetUserById(int id)
        {
            try
            {
                var user = await _usersService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                _logger.LogInformation($"User with ID {id} retrieved successfully.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UsersDTO>> CreateUser([FromBody] CreateUsersDTO dto)
        {
            try
            {
                var createdUser = await _usersService.CreateUserAsync(dto);
                _logger.LogInformation("User created successfully.");
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UsersDTO>> UpdateUser(int id, [FromBody] UpdateUsersDTO dto)
        {
            try
            {
                var updatedUser = await _usersService.UpdateUserAsync(id, dto);
                if (updatedUser == null)
                {
                    _logger.LogWarning($"User with ID {id} not found for update.");
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                _logger.LogInformation($"User with ID {id} updated successfully.");
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                await _usersService.DeleteUserAsync(id);
                _logger.LogInformation($"User with ID {id} deleted successfully.");
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
