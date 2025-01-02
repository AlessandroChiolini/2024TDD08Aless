using System.Collections.Generic;
using System.Linq;
using DAL.Models;
using DAL.Repositories;
using Xunit;

namespace DAL.Tests.Repositories
{
    public class EventRepositoryTests
    {
        private readonly EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            // Initialize the repository with a mock list of events
            _eventRepository = new EventRepository();

            // Seed the repository with initial mock data
            var events = new List<Event>
            {
                new Event { Id = "E1", Name = "Concert", AvailableTickets = 100, TicketPrice = 50.0m },
                new Event { Id = "E2", Name = "Seminar", AvailableTickets = 0, TicketPrice = 30.0m },
                new Event { Id = "E3", Name = "Workshop", AvailableTickets = 50, TicketPrice = 20.0m }
            };

            // Add events to the private `_events` field using reflection (since it's not public)
            var eventsField = typeof(EventRepository)
                .GetField("_events", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            eventsField?.SetValue(_eventRepository, events);
        }

        [Fact]
        public void GetEventById_ReturnsCorrectEvent_WhenExistingEvent()
        {
            // Act
            var eventItem = _eventRepository.GetEventById("E1");

            // Assert
            Assert.NotNull(eventItem);
            Assert.Equal("E1", eventItem.Id);
            Assert.Equal("Concert", eventItem.Name);
        }

        [Fact]
        public void GetEventById_ReturnsNull_WhenNonExistingEvent()
        {
            // Act
            var eventItem = _eventRepository.GetEventById("E999");

            // Assert
            Assert.Null(eventItem);
        }

        [Fact]
        public void GetAvailableEvents_ReturnsOnlyEventsWithTickets_WhenAvailableTickets()
        {
            // Act
            var availableEvents = _eventRepository.GetAvailableEvents();

            // Assert
            Assert.Equal(2, availableEvents.Count); // Only 2 events have available tickets
            Assert.All(availableEvents, e => Assert.True(e.AvailableTickets > 0));
        }

        [Fact]
        public void UpdateEvent_UpdatesEventDetailsCorrectly_WhenExistingEvent()
        {
            // Arrange
            var updatedEvent = new Event
            {
                Id = "E1",
                Name = "Updated Concert",
                AvailableTickets = 80,
                TicketPrice = 45.0m
            };

            // Act
            _eventRepository.UpdateEvent(updatedEvent);

            // Assert
            var eventItem = _eventRepository.GetEventById("E1");
            Assert.NotNull(eventItem);
            Assert.Equal("Updated Concert", eventItem.Name);
            Assert.Equal(80, eventItem.AvailableTickets);
            Assert.Equal(45.0m, eventItem.TicketPrice);
        }

        [Fact]
        public void UpdateEvent_DoesNothing_WhenNonExistingEvent()
        {
            // Arrange
            var nonExistentEvent = new Event
            {
                Id = "E999",
                Name = "Non-existent Event",
                AvailableTickets = 10,
                TicketPrice = 15.0m
            };

            // Act
            _eventRepository.UpdateEvent(nonExistentEvent);

            // Assert
            var eventItem = _eventRepository.GetEventById("E999");
            Assert.Null(eventItem); // No event should be added or updated
        }
    }
}
