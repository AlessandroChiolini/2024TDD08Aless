using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
    public class ListEventsCommandTests
    {
        private const string BaseUri = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_PrintsAvailableEvents_WhenResponseIsSuccessful()
        {
            // Arrange
            var mockEvents = new List<EventDto>
            {
                new EventDto { Id = "1", Name = "Concert", TicketPrice = 50.0m, AvailableTickets = 100 },
                new EventDto { Id = "2", Name = "Workshop", TicketPrice = 30.0m, AvailableTickets = 50 }
            };
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(mockEvents))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new ListEventsCommand(httpClient);

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
        public async Task ExecuteAsync_PrintsNoEventsMessage_WhenNoEventsAreAvailable()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]") // Empty events list
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new ListEventsCommand(httpClient);

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
        public async Task ExecuteAsync_PrintsErrorMessage_WhenServerReturnsError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new ListEventsCommand(httpClient);

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
        public async Task ExecuteAsync_PrintsExceptionMessage_WhenHttpClientThrowsException()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new ListEventsCommand(httpClient);

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
    }
}
