using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleApp.DTOs; // Add namespace reference

namespace ConsoleApp.Commands
{
    public class ListEventsCommand : ICommand
    {
        private readonly HttpClient _httpClient;

        public ListEventsCommand(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                Console.WriteLine("Fetching available events...");

                var response = await _httpClient.GetAsync("api/EventTicket/events");

                if (response.IsSuccessStatusCode)
                {
                    var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();

                    if (events == null || events.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("No events are currently available.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Available Events:");
                        foreach (var evt in events)
                        {
                            Console.WriteLine($"ID: {evt.Id}, Name: {evt.Name}, Price: {evt.TicketPrice:C}, Tickets Available: {evt.AvailableTickets}");
                        }
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to retrieve events. Server returned: {response.StatusCode}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while retrieving events: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
