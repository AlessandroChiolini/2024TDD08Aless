using Business.Services;
using DAL.Models;
using DAL.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.Services
{
    public class BalanceUserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserBalanceService _userBalanceService;

        // Predefined test user to avoid incomplete objects
        private readonly User _testUser = new User
        {
            Id = 1,
            Name = "Alessandro Chiolini",
            Balance = 1500m,
            ServiceCard = "CARD123456"
        };

        public BalanceUserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userBalanceService = new UserBalanceService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserBalanceAsync_ReturnsCorrectBalance_WhenUserExists()
        {
            // Arrange
            var userId = _testUser.Id;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync(_testUser);

            // Act
            var result = await _userBalanceService.GetUserBalanceAsync(userId);

            // Assert
            Assert.Equal(1500m, result);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserBalanceAsync_ReturnsZero_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = _testUser.Id;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userBalanceService.GetUserBalanceAsync(userId);

            // Assert
            Assert.Equal(0m, result);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task AddBalanceAsync_IncreasesBalance_WhenUserExists()
        {
            // Arrange
            var userId = _testUser.Id;
            var amountToAdd = 25.00m;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync(_testUser);

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(_testUser))
                               .Returns(Task.CompletedTask);

            // Act
            await _userBalanceService.AddBalanceAsync(userId, amountToAdd);

            // Assert
            Assert.Equal(1525.00m, _testUser.Balance);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(_testUser), Times.Once);
        }

        [Fact]
        public async Task AddBalanceAsync_DoesNothing_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = _testUser.Id;
            var amountToAdd = 25.00m;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync((User)null);

            // Act
            await _userBalanceService.AddBalanceAsync(userId, amountToAdd);

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = _testUser.Id;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync(_testUser);

            // Act
            var result = await _userBalanceService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testUser.Id, result.Id);
            Assert.Equal(_testUser.Name, result.Name);
            Assert.Equal(_testUser.Balance, result.Balance);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = _testUser.Id;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userBalanceService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
        }
    }
}
