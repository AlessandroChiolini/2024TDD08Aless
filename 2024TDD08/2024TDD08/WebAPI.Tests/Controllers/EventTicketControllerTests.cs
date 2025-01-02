using DAL.Databases;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_API.Controllers;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.Tests.Controllers
{
    public class EventTicketControllerTests
    {
        private readonly AppDbContext _context;
        private readonly EventTicketController _controller;

        public EventTicketControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new EventTicketController(_context);

            SeedData(_context);
        }

        private void SeedData(AppDbContext context)
        {
            context.Users.AddRange(
                new User { Id = 1, Name = "User One", Balance = 1000, ServiceCard = "CARD123" },
                new User { Id = 2, Name = "User Two", Balance = 800, ServiceCard = "CARD456" },
                new User { Id = 3, Name = "User Three", Balance = 1200, ServiceCard = "CARD789" }
            );

            context.Events.AddRange(
                new Event { Id = "E1", Name = "Event One", TicketPrice = 50, AvailableTickets = 100 },
                new Event { Id = "E2", Name = "Event Two", TicketPrice = 30, AvailableTickets = 50 }
            );

            context.EventTickets.AddRange(
                new EventTicket { Id = 1, UserId = 1, EventId = "E1", Quantity = 2, PurchaseDate = DateTime.UtcNow }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEvents_ReturnsAllEvents()
        {
            var result = await _controller.GetAllEvents();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, events.Count());
        }

        [Fact]
        public async Task GetAllEvents_ReturnsNotFound_WhenNoEventsExist()
        {
            // Arrange
            _context.Events.RemoveRange(_context.Events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No events available.", notFoundResult.Value);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            var request = new PurchaseRequest { UserId = 1, EventId = "NonExistent", Quantity = 1 };

            // Act
            var result = await _controller.PurchaseTicket(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Event not found.", notFoundResult.Value);
        }

        [Fact]
        public void GetUserTickets_ReturnsNotFound_WhenNoTicketsExist()
        {
            // Act
            var result = _controller.GetUserTickets(999); // Non-existent user

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No tickets found for this user.", notFoundResult.Value);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsOk_WhenPurchaseIsSuccessful()
        {
            var request = new PurchaseRequest
            {
                UserId = 1,
                EventId = "E2",
                Quantity = 2
            };

            var result = await _controller.PurchaseTicket(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Ticket purchased successfully!", okResult.Value);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsBadRequest_WhenUserDoesNotHaveEnoughBalance()
        {
            var request = new PurchaseRequest
            {
                UserId = 2,
                EventId = "E1",
                Quantity = 50
            };

            var result = await _controller.PurchaseTicket(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient balance.", badRequestResult.Value);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsBadRequest_WhenPayloadIsInvalid()
        {
            // Arrange
            var invalidRequest = new PurchaseRequest { UserId = 1, EventId = "E2", Quantity = 0 };

            // Act
            var result = await _controller.PurchaseTicket(invalidRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsBadRequest_WhenNotEnoughTicketsAvailable()
        {
            // Arrange
            var request = new PurchaseRequest { UserId = 1, EventId = "E1", Quantity = 200 };

            // Act
            var result = await _controller.PurchaseTicket(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Not enough tickets available for this event.", badRequestResult.Value);
        }


        [Fact]
        public void GetUserTickets_ReturnsTickets_WhenTicketsExist()
        {
            var result = _controller.GetUserTickets(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tickets = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(tickets);
        }

        [Fact]
        public async Task GetEventBuyers_ReturnsBuyers_WhenBuyersExist()
        {
            var result = await _controller.GetEventBuyers("E1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var buyers = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(buyers);
        }

        [Fact]
        public async Task GetEventBuyers_ReturnsNotFound_WhenNoBuyersExist()
        {
            var result = await _controller.GetEventBuyers("E3");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No users have purchased tickets for this event.", notFoundResult.Value);
        }

        [Fact]
        public async Task RemoveTicket_ReturnsOk_WhenRemovalIsSuccessful()
        {
            // Arrange
            var eventId = "E2";
            var ticket = new EventTicket { Id = 2, UserId = 1, EventId = eventId, Quantity = 3 };

            _context.ChangeTracker.Clear();
            _context.EventTickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RemoveTicket(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RemoveTicketResponse>(okResult.Value);
            Assert.Equal("Ticket removed successfully.", response.Message);
        }

        [Fact]
        public async Task RemoveTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Act
            var result = await _controller.RemoveTicket("E3");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Use fully qualified namespace if there's a conflict
            var response = Assert.IsType<WebAPI.Controllers.ErrorResponse>(notFoundResult.Value);
            Assert.Equal("Ticket not found for the specified event.", response.Message);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new PurchaseRequest { UserId = 999, EventId = "E1", Quantity = 1 }; // Non-existent user

            // Act
            var result = await _controller.PurchaseTicket(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }
    }
}
