using DAL.Models;
using DAL.Databases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventTicketController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventTicketController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint to retrieve all events
        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _context.Events
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.TicketPrice,
                        e.AvailableTickets
                    })
                    .ToListAsync();

                if (events == null || events.Count == 0)
                {
                    return NotFound("No events available.");
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving events: {ex.Message}");
            }
        }

        // Endpoint to purchase tickets
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseTicket([FromBody] PurchaseRequest request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Invalid request payload.");
            }

            // Retrieve the event
            var eventDetails = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == request.EventId);

            if (eventDetails == null)
            {
                return NotFound("Event not found.");
            }

            // Check if enough tickets are available
            if (eventDetails.AvailableTickets < request.Quantity)
            {
                return BadRequest("Not enough tickets available for this event.");
            }

            // Retrieve the user
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Calculate total cost
            var totalCost = eventDetails.TicketPrice * request.Quantity;

            // Check if user has enough balance
            if (user.Balance < totalCost)
            {
                return BadRequest("Insufficient balance.");
            }

            // Deduct user balance and decrement available tickets
            user.Balance -= totalCost;
            eventDetails.AvailableTickets -= request.Quantity;

            // Add the ticket to the EventTickets table
            var newTicket = new EventTicket
            {
                UserId = request.UserId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                PurchaseDate = DateTime.UtcNow
            };
            _context.EventTickets.Add(newTicket);

            // Update the database
            _context.Users.Update(user);
            _context.Events.Update(eventDetails);
            await _context.SaveChangesAsync();

            return Ok("Ticket purchased successfully!");
        }

        [HttpGet("user/{userId}/tickets")]
        public IActionResult GetUserTickets(int userId)
        {
            var tickets = _context.EventTickets
                .Where(ticket => ticket.UserId == userId)
                .Join(
                    _context.Events,  // Join with Events table
                    ticket => ticket.EventId,
                    eventItem => eventItem.Id,
                    (ticket, eventItem) => new
                    {
                        EventId = ticket.EventId,       // Include Event ID
                        EventName = eventItem.Name,     // Event Name
                        EventDate = eventItem.Date,     // Event Date
                        Quantity = ticket.Quantity      // Ticket Quantity
                    }
                )
                .ToList();

            if (tickets == null || tickets.Count == 0)
                return NotFound("No tickets found for this user.");

            return Ok(tickets);
        }
    }

    public class PurchaseRequest
    {
        public int UserId { get; set; }
        public string EventId { get; set; }
        public int Quantity { get; set; }
    }
}
