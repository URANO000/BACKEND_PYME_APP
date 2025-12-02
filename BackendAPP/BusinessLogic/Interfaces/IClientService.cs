

using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.Helper;

namespace BusinessLogic.Interfaces
{
    public interface IClientService
    {
        //SOLID -> Single Responsibility Principle 
        Task<PagedResult<ClientDTO>> GetAllClientsAsync(ClientFilterDTO dto);
        Task<IEnumerable<ClientDTO>> GetAllNonPaged();
        Task<ClientDTO?> GetClientByIdAsync(int id);
        Task<ClientDTO?> CreateClientAsync(CreateClientDTO dto);
        Task<ClientDTO?> UpdateClientAsync(int id, CreateClientDTO dto);
        Task DeleteClientAsync(int id);
    }
}
