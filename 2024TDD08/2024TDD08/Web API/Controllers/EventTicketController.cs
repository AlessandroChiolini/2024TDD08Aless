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
        private readonly bool _simulateException;

        public EventTicketController(AppDbContext context, bool simulateException = false)
        {
            _context = context;
            _simulateException = simulateException;
        }

        // Endpoint to retrieve all events
        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                if (_simulateException) throw new Exception("Simulated exception");

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
                    return NotFound("No events available.");

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

        // Retrieve tickets purchased by a user
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

        // New Endpoint: Retrieve UserId and UserName of ticket buyers for a specific event
        [HttpGet("{eventId}/buyers")]
        public async Task<IActionResult> GetEventBuyers(string eventId)
        {
            try
            {
                if (_simulateException) throw new Exception("Simulated exception");

                var buyers = await _context.EventTickets
                    .Where(ticket => ticket.EventId == eventId)
                    .Join(
                        _context.Users,
                        ticket => ticket.UserId,
                        user => user.Id,
                        (ticket, user) => new
                        {
                            UserId = user.Id,
                            UserName = user.Name,
                            Quantity = ticket.Quantity,
                            PurchaseDate = ticket.PurchaseDate
                        }
                    )
                    .ToListAsync();

                if (buyers == null || buyers.Count == 0)
                    return NotFound("No users have purchased tickets for this event.");

                return Ok(buyers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving buyers.", Error = ex.Message });
            }
        }

            [HttpDelete("event/{eventId}")]
        public async Task<IActionResult> RemoveTicket(string eventId)
        {
            // Check if the ticket exists for the given event
            var ticket = await _context.EventTickets.FirstOrDefaultAsync(t => t.EventId == eventId);
            if (ticket == null)
            {
                return NotFound(new ErrorResponse { Message = "Ticket not found for the specified event." });
            }

            // Retrieve the event details
            var eventDetails = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventDetails != null)
            {
                // Update available tickets for the event
                eventDetails.AvailableTickets += ticket.Quantity;
                _context.Events.Update(eventDetails);
            }

            // Remove the ticket
            _context.EventTickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok(new RemoveTicketResponse { Message = "Ticket removed successfully." });
        }

    }

    public class PurchaseRequest
    {
        public int UserId { get; set; }
        public string EventId { get; set; }
        public int Quantity { get; set; }
    }

    // DTOs for responses
    public class RemoveTicketResponse
    {
        public string Message { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
