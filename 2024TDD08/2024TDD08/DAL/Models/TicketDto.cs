using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    // DTO class to represent the ticket response
    public class TicketDto
    {
        public string EventId { get; set; }    // Added Event ID
        public string EventName { get; set; }  // Event name
        public DateTime EventDate { get; set; } // Event date
        public int Quantity { get; set; }      // Ticket quantity
    }
}
