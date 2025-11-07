using DataAccess.Models.Entities;

namespace DataAccess.Repositories.Users
{
    public interface IUserRepository
    {
        Task <IEnumerable<UsersDA>> GetAllAsync();
        Task<UsersDA?> GetByIdAsync(int id);
        Task CreateAsync(UsersDA user);
        Task UpdateAsync(UsersDA user);
        Task DeleteAsync(UsersDA user);

        //get by email
        Task <UsersDA?> GetByEmailAsync(string email);

    }
}
