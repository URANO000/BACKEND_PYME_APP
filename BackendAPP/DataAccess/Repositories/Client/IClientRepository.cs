using DataAccess.Models.DTOs.Helper;
using DataAccess.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Client
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientDA>> GetAllAsync();
        Task<ClientDA?> GetByIdAsync(int id);
        Task CreateAsync(ClientDA client);
        Task UpdateAsync(ClientDA client);
        Task DeleteAsync(ClientDA client);

        //For pagination...filtering
        Task<PagedResult<ClientDA>> GetFilteredAsync
        (
            string? search,
            string? email,
            string? lastName,
            int pageNumber = 1,
            int pageSize = 10
        );
    }
}
