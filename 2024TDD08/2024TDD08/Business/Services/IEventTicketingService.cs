using DAL.Models;
using System.Collections.Generic;

namespace Business.Services
{
    public interface IEventTicketingService
    {
        // Purchases a ticket for the specified user and event
        bool PurchaseTicket(int userId, string eventId, int quantity, string serviceCard);

        // Retrieves tickets purchased by a specific user
        List<EventTicket> GetUserTickets(int userId);

        // Retrieves all available events
        List<Event> GetAvailableEvents();

        // Retrieves details of a specific event by its ID
        Event GetEventDetails(string eventId);

        // Cancels a ticket purchase for a user and restores availability
        bool CancelTicket(int userId, string eventId);
    }
}
