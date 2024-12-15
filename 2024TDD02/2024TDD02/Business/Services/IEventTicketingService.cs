using DAL.Models;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public interface IEventTicketingService
    {
        bool PurchaseTicket(int userId, string eventId, int quantity);
        List<EventTicket> GetUserTickets(int userId);
        List<Event> GetAvailableEvents();
        Event GetEventDetails(string eventId);
    }
}
