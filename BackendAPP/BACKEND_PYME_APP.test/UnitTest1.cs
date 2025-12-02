using BackendAPP.Controllers;
using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Client;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace BACKEND_PYME_APP.test
{
    public class ClientControllerTests
    {
        private readonly IClientService _fakeClientService;
        private readonly ILogger<ClientController> _fakeLogger;
        private readonly ClientController _controller;

        //Constructor
        public ClientControllerTests()
        {
            //Arrange this too
            _fakeClientService = A.Fake<IClientService>();
            _fakeLogger = A.Fake<ILogger<ClientController>>();
            _controller = new ClientController(_fakeClientService, _fakeLogger);

        }


        [Fact]
        public async Task GetByClientId_WithValidId_ReturnsOkResultWithClient()
        {
            //Arrange
            //Fake id to test
            int fakeId = 3;

            //Fake client to test my method
            var fakeClientDTO = new ClientDTO
            {
                ClientId = fakeId,
                Cedula = "702770838",
                FirstName = "Patito",
                LastName = "Junior",
                Email = "pjunior@gmail.com",
                Phone = "+506 8878-1949",
                Address = "Un lago, Puntarenas"

            };

            /*Call to service
            What am I doing? I am simulating how I would call thatmethod. I have to return a Task because my operations are async
            I have to look exactly at the params and method signature of my method itself (like the fact it returns a ClientDTO? (nullable)
            */
            A.CallTo(() => _fakeClientService.GetClientByIdAsync(fakeId))
                .Returns(Task.FromResult<ClientDTO?>(fakeClientDTO));

            //Act -- Because this is async!
            var actionResult = await _controller.GetByClientId(fakeId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedClient = Assert.IsType<ClientDTO>(okResult.Value);

            Assert.Equal(fakeId, returnedClient.ClientId);
            Assert.Equal("Patito", returnedClient.FirstName);
            Assert.Equal("Junior", returnedClient.LastName);
        }
    }
}