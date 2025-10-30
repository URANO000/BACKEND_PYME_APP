using BusinessLogic.Services;
using DataAccess;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        //First I call dbConext to use it in my methods
        private readonly ClientService _clientService;
        public ClientController(ClientService clientService)
        {
            _clientService = clientService;
        }

        //Now I can create my HTTP methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetAllClients()
        {
            var client = await _clientService.GetAllClientsAsync();
            return Ok(client);
        }

        //Get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetByClientId(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            //Little validation
            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(client);

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
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        //Now we're going to make our update method
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateClient(int id, [FromBody] CreateClientDTO updatedClient)
        {
            try
            {
                var updated = await _clientService.UpdateClientAsync(id, updatedClient);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        //Finally, we make the delete method
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return Ok(new { message = "Cliente eliminado!" });

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
        }
    }
}
