using DAL.Databases; // Add this to use AppDbContext
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTicketController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventTicketController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("purchase")]
        public IActionResult PurchaseTicket([FromBody] PurchaseRequest request)
        {
            // This still uses the service for purchasing logic
            var success = _context.EventTickets.Add(new DAL.Models.EventTicket
            {
                UserId = request.UserId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                PurchaseDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            return Ok("Ticket purchased successfully");
        }

        [HttpGet("user/{userId}/tickets")]
        public IActionResult GetUserTickets(int userId)
        {
            var tickets = _context.EventTickets
                .Where(ticket => ticket.UserId == userId)
                .ToList();
            return Ok(tickets);
        }

        [HttpGet("events")]
        public IActionResult GetAvailableEvents()
        {
            // Fetch directly from the Events table in the database
            var events = _context.Events.ToList();
            return Ok(events);
        }
    }

    public class PurchaseRequest
    {
        public int UserId { get; set; }
        public string EventId { get; set; }
        public int Quantity { get; set; }
    }
}
