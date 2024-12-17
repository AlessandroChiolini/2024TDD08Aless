using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web_API.Controllers;

namespace WebAPITests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserBalanceService> _userBalanceServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userBalanceServiceMock = new Mock<IUserBalanceService>();
            _userController = new UserController(_userBalanceServiceMock.Object);
        }

        [Fact]
        public async Task GetUserBalance_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            decimal expectedBalance = 100m;
            _userBalanceServiceMock.Setup(service => service.GetUserBalanceAsync(userId)).ReturnsAsync(expectedBalance);

            // Act
            var result = await _userController.GetUserBalance(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedBalance, okResult.Value);
        }

        [Fact]
        public async Task GetUserBalance_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            _userBalanceServiceMock.Setup(service => service.GetUserBalanceAsync(userId)).ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _userController.GetUserBalance(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }

        [Fact]
        public async Task AddBalance_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            decimal amount = 50m;

            // Act
            var result = await _userController.AddBalance(userId, amount);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userBalanceServiceMock.Verify(service => service.AddBalanceAsync(userId, amount), Times.Once);
        }

        [Fact]
        public async Task AddBalance_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            decimal amount = 50m;
            _userBalanceServiceMock.Setup(service => service.AddBalanceAsync(userId, amount)).ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _userController.AddBalance(userId, amount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }
    }
}
