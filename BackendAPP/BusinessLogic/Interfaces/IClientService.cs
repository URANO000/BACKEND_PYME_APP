

using DataAccess.Models.DTOs.Client;

namespace BusinessLogic.Interfaces
{
    public interface IClientService
    {
        //SOLID -> Single Responsibility Principle 
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<ClientDTO?> GetClientByIdAsync(int id);
        Task<ClientDTO?> CreateClientAsync(CreateClientDTO dto);
        Task<ClientDTO?> UpdateClientAsync(int id, CreateClientDTO dto);
        Task DeleteClientAsync(int id);
    }
}
