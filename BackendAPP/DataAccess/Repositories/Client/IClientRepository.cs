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
        Task CreateAsync(ClientDA product);
        Task UpdateAsync(ClientDA product);
        Task DeleteAsync(ClientDA product);
    }
}
