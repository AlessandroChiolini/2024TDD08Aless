using System.Threading.Tasks;
using DAL.Databases;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DAL.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserRepositoryTests()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new AppDbContext(options);

            // Seed initial data
            SeedDatabase();

            // Initialize the repository
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsCorrectUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;

            // Act
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
            Assert.Equal("Alessandro Chiolini", user.Name); // Match seeded data
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 99; // Non-existent user

            // Act
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUserBalanceSuccessfully()
        {
            // Arrange
            var userId = 1;
            var user = await _userRepository.GetUserByIdAsync(userId);
            var newBalance = 2000m;

            // Act
            user.Balance = newBalance;
            await _userRepository.UpdateUserAsync(user);
            var updatedUser = await _userRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(newBalance, updatedUser.Balance);
        }

        [Fact]
        public async Task UpdateUserAsync_DoesNothing_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User
            {
                Id = 99, // Non-existent user
                Name = "Test User",
                Balance = 500m,
                ServiceCard = "TESTCARD123"
            };

            // Act
            await _userRepository.UpdateUserAsync(user);

            // Assert
            var updatedUser = await _userRepository.GetUserByIdAsync(user.Id);
            Assert.Null(updatedUser); // User should not exist
        }

        private void SeedDatabase()
        {
            // Seed initial users
            _context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Name = "Alessandro Chiolini",
                    Balance = 1500,
                    ServiceCard = "CARD123456"
                },
                new User
                {
                    Id = 2,
                    Name = "Julien Blanch-Lanao",
                    Balance = 1000,
                    ServiceCard = "CARD654321"
                },
                new User
                {
                    Id = 3,
                    Name = "Gian-Luca Gloor",
                    Balance = 1200,
                    ServiceCard = "CARD987654"
                }
            );

            _context.SaveChanges();
        }
    }
}
