using DAL.Models;

namespace _2024TDD02.Models
{
    public class CopyTransactionTests
    {
        [Fact]
        public void Constructor_WithAllParameters_ShouldSetProperties()
        {
            // Arrange
            int id = 1;
            int userId = 2;
            int numberOfCopies = 3;
            decimal amount = 4.00m;
            DateTime date = DateTime.Now;

            // Act
            var copyTransaction = new CopyTransaction(id, userId, numberOfCopies, amount, date);

            // Assert
            Assert.Equal(id, copyTransaction.Id);
            Assert.Equal(userId, copyTransaction.UserId);
            Assert.Equal(numberOfCopies, copyTransaction.NumberOfCopies);
            Assert.Equal(amount, copyTransaction.Amount);
            Assert.Equal(date, copyTransaction.Date);
        }

        [Fact]
        public void Constructor_WithoutId_ShouldSetProperties()
        {
            // Arrange
            int userId = 2;
            int numberOfCopies = 3;
            decimal amount = 4.00m;
            DateTime date = DateTime.Now;

            // Act
            var copyTransaction = new CopyTransaction(userId, numberOfCopies, amount, date);

            // Assert
            Assert.Equal(userId, copyTransaction.UserId);
            Assert.Equal(numberOfCopies, copyTransaction.NumberOfCopies);
            Assert.Equal(amount, copyTransaction.Amount);
            Assert.Equal(date, copyTransaction.Date);
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var copyTransaction = new CopyTransaction();

            // Assert
            Assert.Equal(0, copyTransaction.Id);
            Assert.Equal(0, copyTransaction.UserId);
            Assert.Equal(0, copyTransaction.NumberOfCopies);
            Assert.Equal(0.00m, copyTransaction.Amount);
            Assert.Equal(default, copyTransaction.Date);
        }
    }
}