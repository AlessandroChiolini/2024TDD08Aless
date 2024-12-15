using DAL.Databases;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024TDD02.Repositories
{
    public class UserRepositoryTests
    {
        private async Task<AppDbContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CopyPaymentDb")
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureDeleted(); 
            context.Database.EnsureCreated(); 

            if (!context.Users.Any())
            {
                context.Users.AddRange(AppDbContext.UserList);
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var context = await GetInMemoryDbContext();
            var repository = new UserRepository(context);

            // Act
            var user = await repository.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("Alexandre Salamin", user.Name);
            Assert.Equal("alexandre.salamin@mail.com", user.Email);
            Assert.Equal(50, user.Balance);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var context = await GetInMemoryDbContext();
            var repository = new UserRepository(context);
            var user = await repository.GetUserByIdAsync(1);
            user.Balance = 200;

            // Act
            await repository.UpdateUserAsync(user);
            var updatedUser = await repository.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(200, updatedUser.Balance);
        }
    }
}
