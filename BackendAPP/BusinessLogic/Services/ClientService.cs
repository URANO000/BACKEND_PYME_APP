using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.Helper;
using DataAccess.Models.Entities;
using DataAccess.Repositories.Client;


namespace BusinessLogic.Services
{
    public class ClientService : IClientService
    {
        //First inject the repository
        private readonly IClientRepository _clientRepository;

        //Constructor
        public ClientService(IClientRepository clientRespository)
        {
            _clientRepository = clientRespository;
        }

        //Implement the methods by calling the repository methods
        public async Task<PagedResult<ClientDTO>> GetAllClientsAsync(ClientFilterDTO filters)
        {
            var pagedClients = await _clientRepository.GetFilteredAsync(
                filters.Search,
                filters.Email,
                filters.LastName,
                filters.PageNumber,
                filters.PageSize
                );

            return new PagedResult<ClientDTO>
            {
                Items = pagedClients.Items.Select(c => new ClientDTO
                {
                    ClientId = c.ClientId,
                    Cedula = c.Cedula,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address
                }),
                TotalCount = pagedClients.TotalCount,
                PageNumber = pagedClients.PageNumber,
                PageSize = pagedClients.PageSize
            };
        }

        //Implement get by id
        public async Task<ClientDTO?> GetClientByIdAsync(int id)
        {
            //Call the repo method
            var client = await _clientRepository.GetByIdAsync(id);

            //Check and validate if client is null
            if (client == null)
            {
                throw new ArgumentNullException("Cliente no encontrado");
            }

            //If not null, map to DTO and return
            return new ClientDTO
            {
                ClientId = client.ClientId,
                Cedula = client.Cedula,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };


        }

        //Now we do the create method, my least favorite because it's long and my brain hurts
        public async Task<ClientDTO> CreateClientAsync(CreateClientDTO clientDTO)
        {
            //first make sure my dto is not null
            if (clientDTO == null)
            {
                throw new ArgumentNullException("Datos de cliente inválidos.");
            }

            //Now map DTO to entity in order to save to my DB
            var client = new ClientDA
            {
                Cedula = clientDTO.Cedula,
                FirstName = clientDTO.FirstName,
                LastName = clientDTO.LastName,
                Email = clientDTO.Email,
                Phone = clientDTO.Phone,
                Address = clientDTO.Address
            };

            //Now that we have this, we can create it and save the changes
            await _clientRepository.CreateAsync(client);

            //Now we can convert back to DTO for response
            var clientResponse = new ClientDTO
            {
                ClientId = client.ClientId,
                Cedula = client.Cedula,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };

            return clientResponse;
        }

        //Update method, second least favorite
        public async Task<ClientDTO> UpdateClientAsync(int id,CreateClientDTO updatedClient)
        {
            //Verify that the id matches with an actual client
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                throw new KeyNotFoundException("No existe un cliente con ese id!");
            }

            //Verify that the obj given isn't null either
            if(updatedClient == null)
            {
                throw new ArgumentNullException("El cliente no tiene datos válidos");
            }

            //Now that we've verified everything, we have to update the values of client
            client.FirstName = updatedClient.FirstName;
            client.LastName = updatedClient.LastName;
            client.Cedula = updatedClient.Cedula;
            client.Email = updatedClient.Email;
            client.Phone = updatedClient.Phone;
            client.Address = updatedClient.Address;

            //Now we need a DTO to return
            await _clientRepository.UpdateAsync(client);

            return new ClientDTO
            {
                ClientId = client.ClientId,
                Cedula = client.Cedula,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };
        }

        //Finally, delete
        public async Task DeleteClientAsync(int id)
        {
            //Verify the client exists
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                throw new KeyNotFoundException("No existe un cliente con ese id!");
            }
            
            //If true, Delete 
            await _clientRepository.DeleteAsync(client);
        }

    }
}