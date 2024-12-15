using Business.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web_API.Controllers;

namespace WebAPITests.Controllers
{
    public class CopyPaymentControllerTests
    {
        private readonly Mock<ICopyPaymentService> _copyPaymentServiceMock;
        private readonly CopyPaymentController _copyPaymentController;

        public CopyPaymentControllerTests()
        {
            _copyPaymentServiceMock = new Mock<ICopyPaymentService>();
            _copyPaymentController = new CopyPaymentController(_copyPaymentServiceMock.Object);
        }

        [Fact]
        public void GetUserCopyTransactions_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<CopyTransaction>
            {
                new CopyTransaction { Id = 1, UserId = userId, NumberOfCopies = 10, Amount = 2m, Date = System.DateTime.Now },
                new CopyTransaction { Id = 2, UserId = userId, NumberOfCopies = 5, Amount = 1m, Date = System.DateTime.Now }
            };
            _copyPaymentServiceMock.Setup(service => service.GetUserCopyTransactions(userId)).Returns(transactions);

            // Act
            var result = _copyPaymentController.GetUserCopyTransactions(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CopyTransaction>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public void GetUserCopyTransactions_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            _copyPaymentServiceMock.Setup(service => service.GetUserCopyTransactions(userId)).Throws(new System.Exception("Test exception"));

            // Act
            var result = _copyPaymentController.GetUserCopyTransactions(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }

        [Fact]
        public async Task ProcessCopyPayment_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            _copyPaymentServiceMock.Setup(service => service.ProcessCopyPaymentAsync(userId, numberOfCopies)).ReturnsAsync(true);

            // Act
            var result = await _copyPaymentController.ProcessCopyPayment(userId, numberOfCopies);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment processed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ProcessCopyPayment_InsufficientFunds_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            _copyPaymentServiceMock.Setup(service => service.ProcessCopyPaymentAsync(userId, numberOfCopies)).ReturnsAsync(false);

            // Act
            var result = await _copyPaymentController.ProcessCopyPayment(userId, numberOfCopies);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Your account does not have sufficient funds for the payment. Press 4 to add balance.", badRequestResult.Value);
        }

        [Fact]
        public async Task ProcessCopyPayment_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            int numberOfCopies = 10;
            _copyPaymentServiceMock.Setup(service => service.ProcessCopyPaymentAsync(userId, numberOfCopies)).ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _copyPaymentController.ProcessCopyPayment(userId, numberOfCopies);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }
    }
}