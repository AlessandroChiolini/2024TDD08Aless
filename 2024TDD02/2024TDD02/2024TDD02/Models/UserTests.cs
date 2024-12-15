using DAL.Models;
using Xunit;

namespace _2024TDD02.Models
{
    public class UserTests
    {
        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            int id = 1;
            string name = "Alexandre Salamin";
            string email = "alexandre.salamin@mail.com";
            decimal balance = 50;

            // Act
            var user = new User
            {
                Id = id,
                Name = name,
                Email = email,
                Balance = balance
            };

            // Assert
            Assert.Equal(id, user.Id);
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.Equal(balance, user.Balance);
        }

        [Fact]
        public void Constructor_WithRequiredProperties_ShouldSetProperties()
        {
            // Arrange
            string name = "Alexandre Salamin";
            string email = "alexandre.salamin@mail.com";
            decimal balance = 100;

            // Act
            var user = new User
            {
                Name = name,
                Email = email,
                Balance = balance
            };

            // Assert
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.Equal(balance, user.Balance);
        }
    }
}
