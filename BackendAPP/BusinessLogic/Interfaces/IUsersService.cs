using DataAccess.Models.DTOs.User;

namespace BusinessLogic.Interfaces
{
    public interface IUsersService
    {
        //SOLID -> Single Responsibility Principle 
        //Get all users
        Task<IEnumerable<UsersDTO>> GetAllUsersAsync();
        //Get user by id
        Task<UsersDTO?> GetUserByIdAsync(int id);
        //Create user
        Task<UsersDTO?> CreateUserAsync(CreateUsersDTO dto);
        //Update user
        Task<UsersDTO?> UpdateUserAsync(int id, UpdateUsersDTO dto);
        //Delete user
        Task DeleteUserAsync(int id);
    }
}
