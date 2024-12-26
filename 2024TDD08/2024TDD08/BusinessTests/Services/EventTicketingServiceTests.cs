using Business.Services;
using DAL.Models;
using DAL.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.Services
{
    public class EventTicketingServiceTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IEventTicketRepository> _ticketRepositoryMock;
        private readonly EventTicketingService _eventTicketingService;

        public EventTicketingServiceTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketRepositoryMock = new Mock<IEventTicketRepository>();
            _eventTicketingService = new EventTicketingService(_eventRepositoryMock.Object, _ticketRepositoryMock.Object);
        }

        [Fact]
        public void PurchaseTicket_ReturnsTrue_WhenTicketsAreAvailable()
        {
            // Arrange
            var userId = 1;
            var eventId = "E1";
            var quantity = 2;

            var mockEvent = new Event
            {
                Id = eventId,
                Name = "Sample Event",
                AvailableTickets = 10,
                Date = DateTime.UtcNow
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId)).Returns(mockEvent);
            _ticketRepositoryMock.Setup(repo => repo.GetNextId()).Returns(1);

            // Act
            var result = _eventTicketingService.PurchaseTicket(userId, eventId, quantity);

            // Assert
            Assert.True(result);
            _ticketRepositoryMock.Verify(repo => repo.AddTicket(It.IsAny<EventTicket>()), Times.Once);
            _eventRepositoryMock.Verify(repo => repo.UpdateEvent(mockEvent), Times.Once);
        }

        [Fact]
        public void PurchaseTicket_ReturnsFalse_WhenEventDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var eventId = "E1";
            var quantity = 2;

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId)).Returns((Event?)null);

            // Act
            var result = _eventTicketingService.PurchaseTicket(userId, eventId, quantity);

            // Assert
            Assert.False(result);
            _ticketRepositoryMock.Verify(repo => repo.AddTicket(It.IsAny<EventTicket>()), Times.Never);
        }

        [Fact]
        public void PurchaseTicket_ReturnsFalse_WhenTicketsAreNotAvailable()
        {
            // Arrange
            var userId = 1;
            var eventId = "E1";
            var quantity = 10;

            var mockEvent = new Event
            {
                Id = eventId,
                Name = "Sample Event",
                AvailableTickets = 5, // Not enough tickets
                Date = DateTime.UtcNow
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId)).Returns(mockEvent);

            // Act
            var result = _eventTicketingService.PurchaseTicket(userId, eventId, quantity);

            // Assert
            Assert.False(result);
            _ticketRepositoryMock.Verify(repo => repo.AddTicket(It.IsAny<EventTicket>()), Times.Never);
        }

        [Fact]
        public void GetUserTickets_ReturnsTickets_WhenUserHasTickets()
        {
            // Arrange
            var userId = 1;

            var tickets = new List<EventTicket>
            {
                new EventTicket { Id = 1, UserId = userId, EventId = "E1", Quantity = 2 },
                new EventTicket { Id = 2, UserId = userId, EventId = "E2", Quantity = 1 }
            };

            _ticketRepositoryMock.Setup(repo => repo.GetTicketsByUserId(userId)).Returns(tickets);

            // Act
            var result = _eventTicketingService.GetUserTickets(userId);

            // Assert
            Assert.Equal(2, result.Count);
            _ticketRepositoryMock.Verify(repo => repo.GetTicketsByUserId(userId), Times.Once);
        }

        [Fact]
        public void GetUserTickets_ReturnsEmptyList_WhenUserHasNoTickets()
        {
            // Arrange
            var userId = 1;

            _ticketRepositoryMock.Setup(repo => repo.GetTicketsByUserId(userId)).Returns(new List<EventTicket>());

            // Act
            var result = _eventTicketingService.GetUserTickets(userId);

            // Assert
            Assert.Empty(result);
            _ticketRepositoryMock.Verify(repo => repo.GetTicketsByUserId(userId), Times.Once);
        }

        [Fact]
        public void GetAvailableEvents_ReturnsListOfEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = "E1", Name = "Event 1", AvailableTickets = 10 },
                new Event { Id = "E2", Name = "Event 2", AvailableTickets = 5 }
            };

            _eventRepositoryMock.Setup(repo => repo.GetAvailableEvents()).Returns(events);

            // Act
            var result = _eventTicketingService.GetAvailableEvents();

            // Assert
            Assert.Equal(2, result.Count);
            _eventRepositoryMock.Verify(repo => repo.GetAvailableEvents(), Times.Once);
        }

        [Fact]
        public void GetEventDetails_ReturnsEvent_WhenEventExists()
        {
            // Arrange
            var eventId = "E1";
            var mockEvent = new Event
            {
                Id = eventId,
                Name = "Event 1",
                AvailableTickets = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId)).Returns(mockEvent);

            // Act
            var result = _eventTicketingService.GetEventDetails(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            _eventRepositoryMock.Verify(repo => repo.GetEventById(eventId), Times.Once);
        }

        [Fact]
        public void GetEventDetails_ReturnsNull_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = "E1";

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId)).Returns((Event?)null);

            // Act
            var result = _eventTicketingService.GetEventDetails(eventId);

            // Assert
            Assert.Null(result);
            _eventRepositoryMock.Verify(repo => repo.GetEventById(eventId), Times.Once);
        }
    }
}
