using BusinessLogic.Services;
using DataAccess.Models.DTOs.Client;
using Microsoft.AspNetCore.Mvc;


namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class ClientController : ControllerBase
    {
        //First I call dbConext to use it in my methods
        private readonly ClientService _clientService;
        //I call the logger
        private readonly ILogger<ClientController> _logger;
        public ClientController(ClientService clientService, ILogger<ClientController> logger)
        {

            _clientService = clientService;
            _logger = logger;
        }

        //Now I can create my HTTP methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetAllClients(
            [FromQuery] string? search,
            [FromQuery] string? email,
            [FromQuery] string? lastName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {
                var filters = new ClientFilterDTO
                {
                    Search = search,
                    Email = email,
                    LastName = lastName,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                var clients = await _clientService.GetAllClientsAsync(filters);
                _logger.LogInformation("Clients retrieved successfully!");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients");
                return StatusCode(500, new { message = "Error al obtener los clientes, error interno de servidor X_X" });
            }
        }

        //Get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetByClientId(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);

                //Little validation
                if (client == null)
                {
                    _logger.LogWarning($"Client with ID {id} was not found");
                    return NotFound(new { message = "Cliente no encontrado" });
                }
                _logger.LogInformation($"Client with ID {id} retrieved successfully!");
                return Ok(client);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving client with ID {id}");
                return StatusCode(500, new { message = "Error al obtener el cliente, error interno de servidor X_X" });

            }

        }

        //Next we have the post method to create a new client
        [HttpPost]
        public async Task<ActionResult<ClientDTO>> CreateClient([FromBody] CreateClientDTO clientDto)
        {
            //Try and catch cause of argument exceptions
            try
            {
                var result = await _clientService.CreateClientAsync(clientDto);
                return CreatedAtAction(nameof(GetByClientId), new { id = result.ClientId }, result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("Bad request while creating client: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Related entity not found while creating client: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while creating client");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }

        }

        //Now we're going to make our update method
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateClient(int id, [FromBody] CreateClientDTO updatedClient)
        {
            try
            {
                var updated = await _clientService.UpdateClientAsync(id, updatedClient);
                _logger.LogInformation($"Client with ID {id} updated successfully!");
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Client with ID {id} not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning($"Bad request while updating client with ID {id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error while updating client with ID {id}");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }

        }

        //Finally, we make the delete method
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                _logger.LogInformation($"Client with ID {id} deleted successfully!");
                return Ok(new { message = "Cliente eliminado!" });

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Client with ID {id} not found: {ex.Message}");
                return NotFound(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error while deleting client with ID {id}");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }
    }
}
