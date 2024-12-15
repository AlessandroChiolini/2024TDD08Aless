using DAL.Models;
using DAL.Repositories;
using System;

namespace Business.Services
{
    public interface IUserBalanceService
    {
        Task<decimal> GetUserBalanceAsync(int userId);
        Task AddBalanceAsync(int userId, decimal amount);
        Task<User> GetUserByIdAsync(int userId);

    }
    public class UserBalanceService : IUserBalanceService
    {
        private readonly IUserRepository _userRepository;

        public UserBalanceService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.Balance ?? 0;
        }

        public async Task AddBalanceAsync(int userId, decimal amount)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Balance += amount;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user;
        }
    }
}