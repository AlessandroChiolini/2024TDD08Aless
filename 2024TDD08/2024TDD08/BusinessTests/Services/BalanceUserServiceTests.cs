using DAL.Models;
using DAL.Repositories;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Business.Services;

namespace BusinessTests.Services
{
    public class BalanceUserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IUserBalanceService _userBalanceService;

        public BalanceUserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userBalanceService = new UserBalanceService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserBalanceAsync_UserExists_ReturnsBalance()
        {
            // Arrange
            int userId = 1;
            decimal expectedBalance = 100m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = expectedBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var balance = await _userBalanceService.GetUserBalanceAsync(userId);

            // Assert
            Assert.Equal(expectedBalance, balance);
        }

        [Fact]
        public async Task GetUserBalanceAsync_UserDoesNotExist_ReturnsZero()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var balance = await _userBalanceService.GetUserBalanceAsync(userId);

            // Assert
            Assert.Equal(0, balance);
        }

        [Fact]
        public async Task AddBalanceAsync_UserExists_UpdatesBalance()
        {
            // Arrange
            int userId = 1;
            decimal initialBalance = 100m;
            decimal amountToAdd = 50m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = initialBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _userBalanceService.AddBalanceAsync(userId, amountToAdd);

            // Assert
            Assert.Equal(initialBalance + amountToAdd, user.Balance);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task AddBalanceAsync_UserDoesNotExist_DoesNotUpdateBalance()
        {
            // Arrange
            int userId = 1;
            decimal amountToAdd = 50m;
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            await _userBalanceService.AddBalanceAsync(userId, amountToAdd);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
