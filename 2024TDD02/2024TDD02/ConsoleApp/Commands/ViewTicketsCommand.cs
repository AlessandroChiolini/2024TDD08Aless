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
            _httpClient = httpClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Tickets/user/{_userId}");

                if (response.IsSuccessStatusCode)
                {
                    var tickets = await response.Content.ReadFromJsonAsync<List<TicketDto>>();

                    if (tickets == null || tickets.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("No tickets found for your account.");
                        Console.ResetColor();
                        return;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Your Purchased Tickets:");
                    foreach (var ticket in tickets)
                    {
                        Console.WriteLine($"Event: {ticket.EventName}, Date: {ticket.EventDate}, Quantity: {ticket.Quantity}");
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to retrieve tickets. Please try again.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while retrieving tickets: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    public class TicketDto
    {
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int Quantity { get; set; }
    }
}
