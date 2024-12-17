using Business.Services;
using DAL.Models;
using DAL.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BusinessTests.Services
{
    public class CopyPaymentServiceTests
    {
        private readonly Mock<ICopyTransactionRepository<CopyTransaction>> _copyTransactionRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ICopyPaymentService _copyPaymentService;

        public CopyPaymentServiceTests()
        {
            _copyTransactionRepositoryMock = new Mock<ICopyTransactionRepository<CopyTransaction>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _copyPaymentService = new CopyPaymentService(_copyTransactionRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_ValidUserAndSufficientBalance_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            decimal userBalance = 10m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = userBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Once);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_InvalidUserId_ReturnsFalse()
        {
            // Arrange
            int userId = -1;
            int numberOfCopies = 10;

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(It.IsAny<int>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_InsufficientBalance_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            decimal userBalance = 1m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = userBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }


        [Fact]
        public void GetUserCopyTransactions_ValidUserId_ReturnsTransactions()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<CopyTransaction>
            {
                new CopyTransaction { Id = 1, UserId = userId, NumberOfCopies = 10, Amount = 2m, Date = DateTime.Now },
                new CopyTransaction { Id = 2, UserId = userId, NumberOfCopies = 5, Amount = 1m, Date = DateTime.Now }
            };
            _copyTransactionRepositoryMock.Setup(repo => repo.GetUserCopyTransactions(userId)).Returns(transactions);

            // Act
            var result = _copyPaymentService.GetUserCopyTransactions(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(userId, t.UserId));
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            decimal userBalance = 10m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = userBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Never);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task ProcessCopyPaymentAsync_CostPerCopyCalculation_IsCorrect()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            decimal userBalance = 10m;
            var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", Balance = userBalance };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _copyPaymentService.ProcessCopyPaymentAsync(userId, numberOfCopies);

            // Assert
            Assert.True(result);
            decimal expectedAmount = numberOfCopies * 0.20m; 
            Assert.Equal(userBalance - expectedAmount, user.Balance);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
            _copyTransactionRepositoryMock.Verify(repo => repo.Add(It.IsAny<CopyTransaction>()), Times.Once);
            _copyTransactionRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }
    }
}
