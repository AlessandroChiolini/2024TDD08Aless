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
        public async Task<IActionResult> PurchaseTicket([FromBody] PurchaseRequest request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Invalid request payload.");
            }

            // Retrieve the user from the database
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve the event to check the ticket price
            var eventDetails = await _context.Events.FindAsync(request.EventId);
            if (eventDetails == null)
            {
                return NotFound("Event not found.");
            }

            // Calculate total ticket cost
            var totalCost = eventDetails.TicketPrice * request.Quantity;

            // Check if the user has sufficient balance
            if (user.Balance < totalCost)
            {
                return BadRequest("Insufficient balance.");
            }

            // Deduct the balance
            user.Balance -= totalCost;

            // Add the ticket purchase to EventTickets table
            _context.EventTickets.Add(new DAL.Models.EventTicket
            {
                UserId = request.UserId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                PurchaseDate = DateTime.UtcNow
            });

            // Update the user's balance in the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Ticket purchased successfully!");
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
