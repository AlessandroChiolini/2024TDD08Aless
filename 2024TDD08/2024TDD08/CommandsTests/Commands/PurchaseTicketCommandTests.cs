using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Commands;
using ConsoleApp.DTOs;
using Moq;
using Moq.Protected;
using Xunit;

namespace ConsoleApp.Tests.Commands
{
    public class PurchaseTicketCommandTests
    {
        private const string BaseUri = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_PurchasesTicketSuccessfully_WhenValidDetailsAreProvided()
        {
            // Arrange
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;
            var expectedPayload = JsonSerializer.Serialize(new
            {
                UserId = userId,
                EventId = eventId,
                Quantity = ticketCount
            });

            var mockEvent = new EventDto { Id = eventId, Name = "Concert", TicketPrice = 50.0m, AvailableTickets = 100 };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Mock GET request for event details
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<EventDto> { mockEvent }))
                });

            // Mock POST request for ticket purchase
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/purchase") &&
                        VerifyRequestContent(req.Content, expectedPayload)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/purchase") &&
                    VerifyRequestContent(req.Content, expectedPayload)),
                ItExpr.IsAny<CancellationToken>());

            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_FailsToPurchaseTicket_WhenEventDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var eventId = "E2";
            var ticketCount = 2;
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Mock GET request for event details with an empty list
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());

            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/purchase")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_PrintsErrorMessage_WhenServerFails()
        {
            // Arrange
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;
            var mockEvent = new EventDto { Id = eventId, Name = "Concert", TicketPrice = 50.0m, AvailableTickets = 100 };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Mock GET request for event details
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<EventDto> { mockEvent }))
                });

            // Mock POST request for ticket purchase with a failure response
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/purchase")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Insufficient balance.")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/purchase")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_HandlesEventDetailsSuccessfully_WhenEventExists()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;

            // Mock event details fetch
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<EventDto>
                    {
                new EventDto { Id = eventId, Name = "Concert", TicketPrice = 50.0m, AvailableTickets = 100 }
                    }))
                });

            // Mock successful ticket purchase
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/EventTicket/purchase")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_HandlesException_WhenFetchingEventDetailsFails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;

            // Simulate a network error during event details fetch
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_HandlesEmptyEventList_WhenNoEventsExist()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;

            // Simulate an empty event list response
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_PrintsErrorMessage_WhenExceptionIsThrown()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";
            var ticketCount = 2;

            // Mock event details fetch
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<EventDto>
                    {
                new EventDto { Id = eventId, Name = "Concert", TicketPrice = 50.0m, AvailableTickets = 100 }
                    }))
                });

            // Mock POST request to throw an exception
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/EventTicket/purchase")),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Simulated network error during ticket purchase"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount);

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the GET request for event details was made
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());

            // Verify that the POST request was attempted and resulted in an exception
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/EventTicket/purchase")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task FetchEventDetailsAsync_ReturnsNull_WhenHttpRequestFails()
        {
            // Arrange
            var eventId = "E1";
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("api/EventTicket/events")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost:7249/")
            };

            var command = new PurchaseTicketCommand(httpClient, 1, eventId, 2);

            // Act
            var result = await command.FetchEventDetailsAsync(eventId);

            // Assert
            Assert.Null(result);
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains("api/EventTicket/events")),
                ItExpr.IsAny<CancellationToken>());
        }

        // Helper method to verify the request content
        private bool VerifyRequestContent(HttpContent content, string expectedPayload)
        {
            var actualPayload = content.ReadAsStringAsync().Result;
            return actualPayload == expectedPayload;
        }
    }
}
