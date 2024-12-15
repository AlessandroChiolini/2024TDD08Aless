using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class AddUserBalanceCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;
        private readonly decimal _amount;

        public AddUserBalanceCommand(HttpClient httpClient, int userId, decimal amount)
        {
            _httpClient = httpClient;
            _userId = userId;
            _amount = amount;
        }

        public async Task ExecuteAsync()
        {
            var response = await _httpClient.PostAsJsonAsync($"api/User/{_userId}/balance", _amount);
            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Balance added successfully.");
                var newBalance = await _httpClient.GetFromJsonAsync<decimal>($"api/User/{_userId}/balance");
                Console.WriteLine($"New Balance: {newBalance}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to add balance.");
                Console.ResetColor();
            }
        }
    }
}
