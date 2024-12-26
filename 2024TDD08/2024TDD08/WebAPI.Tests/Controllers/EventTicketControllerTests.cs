using DAL.Databases;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.Tests.Controllers
{
    public class EventTicketControllerTests
    {
        private readonly Mock<AppDbContext> _contextMock;
        private readonly EventTicketController _controller;

        public EventTicketControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            var context = new AppDbContext(options);
            _contextMock = new Mock<AppDbContext>();
            _controller = new EventTicketController(context);

            SeedData(context);
        }

        private void SeedData(AppDbContext context)
        {
            // Seed Users
            // Seed Users
            context.Users.AddRange(
                new User { Id = 1, Name = "User One", Balance = 1000, ServiceCard = "CARD123" },
                new User { Id = 2, Name = "User Two", Balance = 800, ServiceCard = "CARD456" },
                new User { Id = 3, Name = "User Three", Balance = 1200, ServiceCard = "CARD789" }
            );

            // Seed Events
            context.Events.AddRange(
                new Event { Id = "E1", Name = "Event One", TicketPrice = 50, AvailableTickets = 100 },
                new Event { Id = "E2", Name = "Event Two", TicketPrice = 30, AvailableTickets = 50 }
            );

            // Seed EventTickets
            context.EventTickets.AddRange(
                new EventTicket { Id = 1, UserId = 1, EventId = "E1", Quantity = 2, PurchaseDate = DateTime.UtcNow }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEvents_ReturnsAllEvents()
        {
            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, events.Count());
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsOk_WhenPurchaseIsSuccessful()
        {
            // Arrange
            var request = new PurchaseRequest
            {
                UserId = 1,
                EventId = "E2",
                Quantity = 2
            };

            // Act
            var result = await _controller.PurchaseTicket(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PurchaseTicket_ReturnsBadRequest_WhenUserDoesNotHaveEnoughBalance()
        {
            // Arrange
            var request = new PurchaseRequest
            {
                UserId = 2,
                EventId = "E1",
                Quantity = 50
            };

            // Act
            var result = await _controller.PurchaseTicket(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient balance.", badRequestResult.Value);
        }

        [Fact]
        public void GetUserTickets_ReturnsTickets_WhenTicketsExist()
        {
            // Act
            var result = _controller.GetUserTickets(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tickets = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(tickets);
        }

        [Fact]
        public async Task GetEventBuyers_ReturnsBuyers_WhenBuyersExist()
        {
            // Act
            var result = await _controller.GetEventBuyers("E1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var buyers = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(buyers);
        }

        [Fact]
        public async Task GetEventBuyers_ReturnsNotFound_WhenNoBuyersExist()
        {
            // Act
            var result = await _controller.GetEventBuyers("E3");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No users have purchased tickets for this event.", notFoundResult.Value);
        }
    }
}
