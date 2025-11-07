using Microsoft.EntityFrameworkCore;
using DataAccess.Models.Entities;

namespace DataAccess.Repositories.Users
{
    public class UsersRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Method implementation goes here
        public async Task<IEnumerable<UsersDA>> GetAllAsync()
            => await _context.Users.Include(u => u.Role).ToListAsync();

        //Get by Id
        public async Task<UsersDA?> GetByIdAsync(int id)
            => await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

        //Create new user
        public async Task CreateAsync(UsersDA user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        //Update new users
        public async Task UpdateAsync(UsersDA user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        //Lastly, delete users
        public async Task DeleteAsync(UsersDA user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        //Get user by email
        public async Task<UsersDA?> GetByEmailAsync(string email)
            => await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
    }
}
