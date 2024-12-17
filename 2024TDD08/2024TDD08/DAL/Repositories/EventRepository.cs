using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly List<Event> _events;

        public EventRepository()
        {
            _events = new List<Event>(); // Mock database
        }

        public Event GetEventById(string eventId)
        {
            return _events.FirstOrDefault(e => e.Id == eventId);
        }

        public List<Event> GetAvailableEvents()
        {
            return _events.Where(e => e.AvailableTickets > 0).ToList();
        }

        public void UpdateEvent(Event eventItem)
        {
            var index = _events.FindIndex(e => e.Id == eventItem.Id);
            if (index >= 0)
            {
                _events[index] = eventItem;
            }
        }
    }

}
