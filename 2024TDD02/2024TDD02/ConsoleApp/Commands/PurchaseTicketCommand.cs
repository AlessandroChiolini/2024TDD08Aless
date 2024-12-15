using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class PurchaseTicketCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;
        private readonly string _eventId;
        private readonly int _ticketCount;

        public PurchaseTicketCommand(HttpClient httpClient, int userId, string eventId, int ticketCount)
        {
            _httpClient = httpClient;
            _userId = userId;
            _eventId = eventId;
            _ticketCount = ticketCount;
        }

        public async Task ExecuteAsync()
        {
            var payload = new
            {
                UserId = _userId,
                EventId = _eventId,
                TicketCount = _ticketCount
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Tickets/purchase", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Tickets purchased successfully!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to purchase tickets. Please check your balance and ticket availability.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while purchasing tickets: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
