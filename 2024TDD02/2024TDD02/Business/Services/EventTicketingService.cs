using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;

namespace Business.Services
{
    public class EventTicketingService : IEventTicketingService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventTicketRepository _ticketRepository;

        public EventTicketingService(IEventRepository eventRepository, IEventTicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
        }

        public bool PurchaseTicket(int userId, string eventId, int quantity)
        {
            var eventItem = _eventRepository.GetEventById(eventId);
            if (eventItem == null || !eventItem.HasAvailableTickets(quantity))
                return false;

            if (eventItem.ReserveTickets(quantity))
            {
                _ticketRepository.AddTicket(new EventTicket
                {
                    Id = _ticketRepository.GetNextId(),
                    UserId = userId,
                    EventId = eventId,
                    Quantity = quantity,
                    PurchaseDate = DateTime.Now
                });
                return true;
            }
            return false;
        }

        public List<EventTicket> GetUserTickets(int userId)
        {
            return _ticketRepository.GetTicketsByUserId(userId);
        }

        public List<Event> GetAvailableEvents()
        {
            return _eventRepository.GetAvailableEvents();
        }

        public Event GetEventDetails(string eventId)
        {
            return _eventRepository.GetEventById(eventId);
        }
    }

}
