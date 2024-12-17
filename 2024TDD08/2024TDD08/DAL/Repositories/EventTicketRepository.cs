using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class EventTicketRepository : IEventTicketRepository
    {
        private readonly List<EventTicket> _tickets;

        public EventTicketRepository()
        {
            _tickets = new List<EventTicket>(); // Mock database
        }

        public int GetNextId()
        {
            return _tickets.Count + 1;
        }

        public void AddTicket(EventTicket ticket)
        {
            _tickets.Add(ticket);
        }

        public List<EventTicket> GetTicketsByUserId(int userId)
        {
            return _tickets.Where(t => t.UserId == userId).ToList();
        }
    }

}
