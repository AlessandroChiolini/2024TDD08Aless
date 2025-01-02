using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp.DTOs; // Add namespace reference

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
            try
            {
                Console.WriteLine("Validating the event details...");

                var eventDetails = await FetchEventDetailsAsync(_eventId);
                if (eventDetails == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Event not found. Please check the Event ID and try again.");
                    Console.ResetColor();
                    return;
                }

                var totalCost = eventDetails.TicketPrice * _ticketCount;
                Console.WriteLine($"Event Found: {eventDetails.Name}");
                Console.WriteLine($"Ticket Price: {eventDetails.TicketPrice:C}, Total Cost: {totalCost:C}");

                var payload = new
                {
                    UserId = _userId,
                    EventId = _eventId,
                    Quantity = _ticketCount
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine("Processing your purchase...");

                var response = await _httpClient.PostAsync("api/EventTicket/purchase", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Tickets purchased successfully!");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to purchase tickets. Please check your balance and ticket availability.");
                    Console.WriteLine($"Error Details: {response.StatusCode}, {errorContent}");
                }
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred during the purchase: {ex.Message}");
                Console.ResetColor();
            }
        }

        public async Task<EventDto> FetchEventDetailsAsync(string eventId)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/EventTicket/events");
                if (response.IsSuccessStatusCode)
                {
                    var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();
                    return events?.Find(e => e.Id == eventId);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error fetching event details: {ex.Message}");
                Console.ResetColor();
            }

            return null;
        }
    }
}
