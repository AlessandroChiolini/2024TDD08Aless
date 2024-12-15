using DAL.Databases;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Repositories
{
    public class CopyTransactionRepositoryTests
    {
        private async Task<AppDbContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CopyPaymentDb")
                .Options;

            var context = new AppDbContext(options);
            if (!context.CopyTransactions.Any())
            {
                context.CopyTransactions.AddRange(AppDbContext.TransactionList);
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task GetUserCopyTransactions_ShouldReturnTransactions()
        {
            // Arrange
            var context = await GetInMemoryDbContext();
            var repository = new CopyTransactionRepository(context);

            // Act
            var transactions = repository.GetUserCopyTransactions(1);

            // Assert
            Assert.NotNull(transactions);
            Assert.Equal(2, transactions.Count); 
            Assert.All(transactions, t => Assert.Equal(1, t.UserId));
        }

        [Fact]
        public async Task Add_ShouldAddTransaction()
        {
            // Arrange
            var context = await GetInMemoryDbContext();
            var repository = new CopyTransactionRepository(context);
            var transaction = new CopyTransaction { Id = 3, UserId = 1, NumberOfCopies = 20, Amount = 4.0m, Date = DateTime.UtcNow };

            // Act
            repository.Add(transaction);
            repository.SaveChanges();
            var transactions = repository.GetUserCopyTransactions(1);
            // Assert
            Assert.NotNull(transactions);
            Assert.Equal(3, transactions.Count);
        }
    }
}
