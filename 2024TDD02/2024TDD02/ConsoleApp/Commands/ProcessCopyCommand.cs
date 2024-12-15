using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class ProcessCopyPaymentCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;
        private readonly int _numberOfCopies;

        public ProcessCopyPaymentCommand(HttpClient httpClient, int userId, int numberOfCopies)
        {
            _httpClient = httpClient;
            _userId = userId;
            _numberOfCopies = numberOfCopies;
        }

        public async Task ExecuteAsync()
        {
            var result = await ProcessCopyPaymentAsync(_httpClient, _userId, _numberOfCopies);
            if (result)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Payment processed successfully.");
                var newBalance = await _httpClient.GetFromJsonAsync<decimal>($"api/User/{_userId}/balance");
                Console.WriteLine($"New Balance: {newBalance}");
                Console.ResetColor();
            }
        }

        private static async Task<bool> ProcessCopyPaymentAsync(HttpClient httpClient, int userId, int numberOfCopies)
        {
            try
            {
                var response = await httpClient.PostAsync($"api/CopyPayment/ProcessCopyPayment?userId={userId}&numberOfCopies={numberOfCopies}", null);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (errorMessage.Contains("insufficient funds", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Your account does not have sufficient funds for the payment.");
                    }
                    else
                    {
                        Console.WriteLine(errorMessage);
                    }
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return false;
            }
        }
    }
}
