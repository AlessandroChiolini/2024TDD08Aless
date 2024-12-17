using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class GetUserBalanceCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;

        public GetUserBalanceCommand(HttpClient httpClient, int userId)
        {
            _httpClient = httpClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            var response = await _httpClient.GetAsync($"api/User/{_userId}/balance");
            if (response.IsSuccessStatusCode)
            {
                var balance = await response.Content.ReadAsStringAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"User Balance: {balance}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to retrieve user balance.");
                Console.ResetColor();
            }
        }
    }
}
