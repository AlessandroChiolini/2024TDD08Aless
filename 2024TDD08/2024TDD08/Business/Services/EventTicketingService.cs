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
        var eventEntity = _eventRepository.GetEventById(eventId);
        if (eventEntity == null || eventEntity.AvailableTickets < quantity)
        {
            return false;
        }

        eventEntity.AvailableTickets -= quantity;
        _eventRepository.UpdateEvent(eventEntity);

        var ticket = new EventTicket
        {
            Id = _ticketRepository.GetNextId(),
            UserId = userId,
            EventId = eventId,
            Quantity = quantity
        };

        _ticketRepository.AddTicket(ticket);
        return true;
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

        public bool PurchaseTicket(int userId, string eventId, int quantity, string serviceCard)
        {
            throw new NotImplementedException();
        }

        public bool CancelTicket(int userId, string eventId)
        {
            throw new NotImplementedException();
        }
    }

}
