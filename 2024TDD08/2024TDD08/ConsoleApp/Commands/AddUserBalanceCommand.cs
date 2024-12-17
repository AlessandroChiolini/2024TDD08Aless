using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
            var payload = new
            {
                UserId = _userId,
                Amount = _amount
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/User/addbalance", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Successfully added {_amount:C} to your balance.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to add balance. Please try again.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while adding balance: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
