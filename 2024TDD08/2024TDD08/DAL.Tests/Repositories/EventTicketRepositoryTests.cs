﻿using System.Collections.Generic;
using System.Linq;
using DAL.Models;
using DAL.Repositories;
using Xunit;

namespace DAL.Tests.Repositories
{
    public class EventTicketRepositoryTests
    {
        private readonly EventTicketRepository _ticketRepository;

        public EventTicketRepositoryTests()
        {
            // Initialize the repository with a mock list of tickets
            _ticketRepository = new EventTicketRepository();

            // Seed the repository with initial mock data
            var tickets = new List<EventTicket>
            {
                new EventTicket { Id = 1, UserId = 1, EventId = "E1", Quantity = 2, PurchaseDate = System.DateTime.Now },
                new EventTicket { Id = 2, UserId = 2, EventId = "E2", Quantity = 1, PurchaseDate = System.DateTime.Now },
                new EventTicket { Id = 3, UserId = 1, EventId = "E3", Quantity = 3, PurchaseDate = System.DateTime.Now }
            };

            // Add tickets to the private `_tickets` field using reflection
            var ticketsField = typeof(EventTicketRepository)
                .GetField("_tickets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ticketsField?.SetValue(_ticketRepository, tickets);
        }

        [Fact]
        public void GetNextId_ReturnsCorrectNextId_WhenExistingTickets()
        {
            // Act
            var nextId = _ticketRepository.GetNextId();

            // Assert
            Assert.Equal(4, nextId); // Next ID should be the count of tickets + 1
        }

        [Fact]
        public void AddTicket_AddsTicketToRepository_WhenValidTicket()
        {
            // Arrange
            var newTicket = new EventTicket
            {
                Id = 4,
                UserId = 3,
                EventId = "E4",
                Quantity = 2,
                PurchaseDate = System.DateTime.Now
            };

            // Act
            _ticketRepository.AddTicket(newTicket);

            // Assert
            var allTickets = GetAllTickets();
            Assert.Contains(newTicket, allTickets);
            Assert.Equal(4, allTickets.Count); // Total tickets should increase to 4
        }

        [Fact]
        public void GetTicketsByUserId_ReturnsCorrectTickets_WhenExistingUser()
        {
            // Act
            var userTickets = _ticketRepository.GetTicketsByUserId(1);

            // Assert
            Assert.NotEmpty(userTickets);
            Assert.Equal(2, userTickets.Count); // User 1 has 2 tickets
            Assert.All(userTickets, t => Assert.Equal(1, t.UserId)); // All tickets belong to user 1
        }

        [Fact]
        public void GetTicketsByUserId_ReturnsEmptyList_WhenNonExistingUser()
        {
            // Act
            var userTickets = _ticketRepository.GetTicketsByUserId(99);

            // Assert
            Assert.Empty(userTickets);
        }

        [Fact]
        public void RemoveTicket_RemovesTicket_WhenExistingTicket()
        {
            // Arrange
            var ticketId = 1;

            // Act
            _ticketRepository.RemoveTicket(ticketId);

            // Assert
            var allTickets = GetAllTickets();
            Assert.DoesNotContain(allTickets, t => t.Id == ticketId); // Ticket should be removed
            Assert.Equal(2, allTickets.Count); // Total tickets should decrease
        }

        [Fact]
        public void RemoveTicket_DoesNothing_WhenNonExistingTicket()
        {
            // Arrange
            var nonExistentTicketId = 99;

            // Act
            _ticketRepository.RemoveTicket(nonExistentTicketId);

            // Assert
            var allTickets = GetAllTickets();
            Assert.Equal(3, allTickets.Count);
        }

        [Fact]
        public void GetTicketById_ReturnsCorrectTicket_WhenExistingTicket()
        {
            // Arrange
            var ticketId = 1;

            // Act
            var result = _ticketRepository.GetTicketById(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.Id);
            Assert.Equal(1, result.UserId);
            Assert.Equal("E1", result.EventId);
        }

        [Fact]
        public void GetTicketById_ReturnsNull_WhenNonExistingTicket()
        {
            // Arrange
            var nonExistentTicketId = 99;

            // Act
            var result = _ticketRepository.GetTicketById(nonExistentTicketId);

            // Assert
            Assert.Null(result);
        }


        // Helper method to retrieve all tickets (for internal verification)
        private List<EventTicket> GetAllTickets()
        {
            var ticketsField = typeof(EventTicketRepository)
                .GetField("_tickets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return ticketsField?.GetValue(_ticketRepository) as List<EventTicket>;
        }
    }
}
