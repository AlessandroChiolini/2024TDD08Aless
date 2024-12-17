using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class ViewTicketsCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;

        public ViewTicketsCommand(HttpClient httpClient, int userId)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                Console.WriteLine("Fetching your purchased tickets...");

                // Send request to retrieve user's tickets
                var response = await _httpClient.GetAsync($"api/EventTicket/user/{_userId}/tickets");

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize tickets response
                    var tickets = await response.Content.ReadFromJsonAsync<List<TicketDto>>();

                    if (tickets == null || tickets.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("No tickets found for your account.");
                        Console.ResetColor();
                        return;
                    }

                    // Display user's tickets
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nYour Purchased Tickets:");
                    Console.ResetColor();

                    foreach (var ticket in tickets)
                    {
                        Console.WriteLine(
                            $"Event ID: {ticket.EventId}, Event: {ticket.EventName}, Date: {ticket.EventDate:yyyy-MM-dd}, Quantity: {ticket.Quantity}");
                    }
                }
                else
                {
                    // Handle response failure
                    Console.ForegroundColor = ConsoleColor.Red;
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Failed to retrieve tickets. Please try again.");
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                // Catch and display errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while retrieving tickets: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // DTO class to represent the ticket response
    public class TicketDto
    {
        public string EventId { get; set; }    // Added Event ID
        public string EventName { get; set; }  // Event name
        public DateTime EventDate { get; set; } // Event date
        public int Quantity { get; set; }      // Ticket quantity
    }
}
