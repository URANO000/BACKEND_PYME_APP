using DataAccess.Models.DTOs.Helper;
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

        //Implement get by filtered async
        public async Task<PagedResult<ClientDA>> GetFilteredAsync(
            string? search,
            string? email,
            string? lastName,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Clients.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Cedula.Contains(search));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(c => c.Email.Contains(email));


            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(c => c.LastName.Contains(lastName));

            var totalCount = await query.CountAsync();

            var clients = await query
                .OrderBy(c => c.ClientId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ClientDA>
            {
                Items = clients,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
