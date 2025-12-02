using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Role;
using DataAccess.Models.DTOs.User;
using DataAccess.Models.Entities;
using DataAccess.Repositories.Users;

namespace BusinessLogic.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        public UsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //implementation of business logic methods calling repository methods
        public async Task<IEnumerable<UsersDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UsersDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = new RoleDTO
                {
                    RoleId = u.Role.RoleId,
                    RoleName = u.Role.RoleName
                }
            });
        }

        //Get by ID method logic
        public async Task<UsersDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return new UsersDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = new RoleDTO
                {
                    RoleId = user.Role.RoleId,
                    RoleName = user.Role.RoleName
                }
            };
        }

        //Create a new User
        public async Task<UsersDTO> CreateUserAsync(CreateUsersDTO newUser)
        {
            //Hash the password here if needed before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password); 
            
            var userEntity = new UsersDA
            {
                Username = newUser.Username,
                Email = newUser.Email,
                Password = hashedPassword,
                RoleId = newUser.RoleId
            };

            await _userRepository.CreateAsync(userEntity);

            return new UsersDTO
            {
                UserId = userEntity.UserId,
                Username = userEntity.Username,
                Email = userEntity.Email,
                Role = new RoleDTO
                {
                    RoleId = userEntity.RoleId,
                    RoleName = userEntity.Role?.RoleName ?? string.Empty
                }
            };
        }

        public async Task<UsersDTO> UpdateUserAsync(int id, UpdateUsersDTO updatedUser)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            existingUser.Username = updatedUser.Username;
            existingUser.Email = updatedUser.Email;
            existingUser.RoleId = updatedUser.RoleId;
            await _userRepository.UpdateAsync(existingUser);
            return new UsersDTO
            {
                UserId = existingUser.UserId,
                Username = existingUser.Username,
                Email = existingUser.Email,
                Role = new RoleDTO
                {
                    RoleId = existingUser.RoleId,
                    RoleName = existingUser.Role?.RoleName ?? string.Empty
                }
            };
        }

        //Now we implement delete
        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }
            await _userRepository.DeleteAsync(user);

        }
    }
}
