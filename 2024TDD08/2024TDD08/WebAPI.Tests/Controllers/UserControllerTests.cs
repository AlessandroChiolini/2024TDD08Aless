using Business.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Web_API.Controllers;
using Xunit;

namespace WebAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserBalanceService> _userBalanceServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userBalanceServiceMock = new Mock<IUserBalanceService>();
            _controller = new UserController(_userBalanceServiceMock.Object);
        }

        [Fact]
        public async Task AddBalance_ReturnsOk_WhenBalanceUpdatedSuccessfully()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;
            var updatedBalance = 600.50m;
            var request = new BalanceRequest { Amount = amount };

            _userBalanceServiceMock.Setup(s => s.AddBalanceAsync(userId, amount))
                                   .Returns(Task.CompletedTask);

            _userBalanceServiceMock.Setup(s => s.GetUserBalanceAsync(userId))
                                   .ReturnsAsync(updatedBalance);

            // Act
            var result = await _controller.AddBalance(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BalanceUpdateResponse>(okResult.Value);
            Assert.Equal("Balance updated successfully.", response.Message);
            Assert.Equal(updatedBalance, response.UpdatedBalance);

            _userBalanceServiceMock.Verify(s => s.AddBalanceAsync(userId, amount), Times.Once);
            _userBalanceServiceMock.Verify(s => s.GetUserBalanceAsync(userId), Times.Once);
        }

        [Fact]
        public async Task AddBalance_ReturnsBadRequest_WhenAmountIsInvalid()
        {
            // Arrange
            var userId = 1;
            var request = new BalanceRequest { Amount = -10.00m };

            // Act
            var result = await _controller.AddBalance(userId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Amount must be greater than 0.", response.Message);

            _userBalanceServiceMock.Verify(s => s.AddBalanceAsync(It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task AddBalance_ReturnsServerError_OnException()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;
            var request = new BalanceRequest { Amount = amount };

            _userBalanceServiceMock.Setup(s => s.AddBalanceAsync(userId, amount))
                                   .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddBalance(userId, request);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            var response = Assert.IsType<ErrorResponse>(serverErrorResult.Value);
            Assert.Equal("Failed to update balance.", response.Message);

            _userBalanceServiceMock.Verify(s => s.AddBalanceAsync(userId, amount), Times.Once);
        }

        [Fact]
        public async Task GetUserBalance_ReturnsCorrectBalance_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var expectedBalance = 500.50m;

            _userBalanceServiceMock.Setup(s => s.GetUserBalanceAsync(userId))
                                   .ReturnsAsync(expectedBalance);

            // Act
            var result = await _controller.GetUserBalance(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserBalanceResponse>(okResult.Value);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(expectedBalance, response.Balance);

            _userBalanceServiceMock.Verify(s => s.GetUserBalanceAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserBalance_ReturnsZero_WhenUserHasNoBalance()
        {
            // Arrange
            var userId = 1;
            var expectedBalance = 0m;

            _userBalanceServiceMock.Setup(s => s.GetUserBalanceAsync(userId))
                                   .ReturnsAsync(expectedBalance);

            // Act
            var result = await _controller.GetUserBalance(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserBalanceResponse>(okResult.Value);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(expectedBalance, response.Balance);

            _userBalanceServiceMock.Verify(s => s.GetUserBalanceAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserBalance_ReturnsBadRequest_OnException()
        {
            // Arrange
            var userId = 1;

            _userBalanceServiceMock.Setup(s => s.GetUserBalanceAsync(userId))
                                   .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetUserBalance(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Failed to fetch balance.", response.Message);

            _userBalanceServiceMock.Verify(s => s.GetUserBalanceAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User
            {
                Id = userId,
                Name = "Test User",
                Balance = 1000.50m,
                ServiceCard = "TEST123"
            };

            _userBalanceServiceMock.Setup(s => s.GetUserByIdAsync(userId))
                                   .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var user = Assert.IsType<User>(okResult.Value);
            Assert.Equal(expectedUser.Id, user.Id);
            Assert.Equal(expectedUser.Name, user.Name);
            Assert.Equal(expectedUser.Balance, user.Balance);

            _userBalanceServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;

            _userBalanceServiceMock.Setup(s => s.GetUserByIdAsync(userId))
                                   .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal($"User with ID {userId} not found.", response.Message);

            _userBalanceServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }
    }
}
