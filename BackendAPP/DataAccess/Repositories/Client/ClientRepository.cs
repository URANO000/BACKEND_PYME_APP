using DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Client
{
    public class ClientRepository : IClientRepository
    {
        //Since we're in the DAL layer, we interact with the DB directly here
        private readonly ApplicationDbContext _context;

        //Constructor
        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Method implementation for BLL
        public async Task<IEnumerable<ClientDA>> GetAllAsync()
            => await _context.Clients.ToListAsync();

        public async Task<ClientDA?> GetByIdAsync(int id)
            => await _context.Clients.FindAsync(id);

        public async Task CreateAsync(ClientDA client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClientDA client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ClientDA client)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}
