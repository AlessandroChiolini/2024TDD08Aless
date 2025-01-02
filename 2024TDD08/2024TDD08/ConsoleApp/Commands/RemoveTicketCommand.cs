using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ConsoleApp.Commands
{
    public class RemoveTicketCommand : ICommand
    {
        private readonly HttpClient _httpClient;
        private readonly int _userId;

        public RemoveTicketCommand(HttpClient httpClient, int userId)
        {
            _httpClient = httpClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                // Prompt the user for an Event ID to remove first
                Console.Write("\nEnter the Event ID to remove: ");
                var eventId = Console.ReadLine();

                if (string.IsNullOrEmpty(eventId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Event ID. Please enter a valid value.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine("Fetching your purchased tickets...");

                // Fetch user's purchased tickets
                var response = await _httpClient.GetAsync($"api/EventTicket/user/{_userId}/tickets");
                if (!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Failed to retrieve tickets.");
                    Console.WriteLine($"Error Details: {response.StatusCode}, {errorContent}");
                    Console.ResetColor();
                    return;
                }

                var tickets = await response.Content.ReadFromJsonAsync<List<UserTicketDto>>();
                if (tickets == null || tickets.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No tickets found for your account.");
                    Console.ResetColor();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nYour Purchased Tickets:");
                Console.ResetColor();

                // Display the list of purchased tickets
                foreach (var ticket in tickets)
                {
                    Console.WriteLine(
                        $"Event ID: {ticket.EventId}, Event: {ticket.EventName}, Quantity: {ticket.Quantity}");
                }

                Console.WriteLine("Processing your ticket removal...");

                // Send DELETE request to remove the ticket using EventId
                var deleteResponse = await _httpClient.DeleteAsync($"api/EventTicket/event/{eventId}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ticket removed successfully!");
                }
                else
                {
                    var errorContent = await deleteResponse.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to remove the ticket.");
                    Console.WriteLine($"Error Details: {deleteResponse.StatusCode}, {errorContent}");
                }

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while removing the ticket: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Updated DTO class for Ticket
    public class UserTicketDto
    {
        public int TicketId { get; set; }       // Ticket ID
        public string EventId { get; set; }    // Event ID
        public string EventName { get; set; }  // Event name
        public int Quantity { get; set; }      // Ticket quantity
    }
}
