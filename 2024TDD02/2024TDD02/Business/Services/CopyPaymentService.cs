using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface ICopyPaymentService
    {
        Task<bool> ProcessCopyPaymentAsync(int userId, int numberOfCopies);
        List<CopyTransaction> GetUserCopyTransactions(int userId);
    }

    public class CopyPaymentService : ICopyPaymentService
    {
        private readonly ICopyTransactionRepository<CopyTransaction> _copyTransactionRepository;
        private readonly IUserRepository _userRepository;
        private const decimal CostPerCopy = 0.20m;
        
        public CopyPaymentService(ICopyTransactionRepository<CopyTransaction> copyTransactionRepository, IUserRepository userRepository)
        {
            _copyTransactionRepository = copyTransactionRepository;
            _userRepository = userRepository;
        }

        public List<CopyTransaction> GetUserCopyTransactions(int userId)
        {
            return _copyTransactionRepository.GetUserCopyTransactions(userId);
        }

        public async Task<bool> ProcessCopyPaymentAsync(int userId, int numberOfCopies)
        {
            // Validate input parameters
            if (userId <= 0 || numberOfCopies <= 0)
            {
                return false;
            }

            // Calculate the amount based on the number of copies
            var amount = numberOfCopies * CostPerCopy;

            // Get the user
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.Balance < amount)
            {
                return false;
            }

            var copyTransaction = new CopyTransaction(
                userId: userId,
                numberOfCopies: numberOfCopies,
                amount: amount,
                date: DateTime.Now
            );

            try
            {
                // Deduct the amount from the user's balance
                user.Balance -= amount;
                await _userRepository.UpdateUserAsync(user);

                // Save the transaction using the repository
                _copyTransactionRepository.Add(copyTransaction);
                _copyTransactionRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception details 
                Console.WriteLine($"Error processing payment: {ex.Message}");
                return false;
            }
        }
    }
}
