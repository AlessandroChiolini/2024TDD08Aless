using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Commands;
using Moq;
using Moq.Protected;
using Xunit;

namespace ConsoleApp.Tests.Commands
{
    public class ViewTicketsCommandTests
    {
        private const string BaseUri = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_DisplaysTickets_WhenResponseIsSuccessful()
        {
            // Arrange
            var userId = 1;
            var tickets = new List<TicketDto>
            {
                new TicketDto
                {
                    EventId = "E1",
                    EventName = "Concert",
                    EventDate = DateTime.UtcNow.AddDays(5),
                    Quantity = 2
                },
                new TicketDto
                {
                    EventId = "E2",
                    EventName = "Tech Workshop",
                    EventDate = DateTime.UtcNow.AddDays(10),
                    Quantity = 1
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(tickets))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new ViewTicketsCommand(httpClient, userId);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysNoTicketsMessage_WhenNoTicketsAreFound()
        {
            // Arrange
            var userId = 1;
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
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

            var command = new ViewTicketsCommand(httpClient, userId);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysErrorMessage_WhenResponseFails()
        {
            // Arrange
            var userId = 1;
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Internal Server Error")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };

            var command = new ViewTicketsCommand(httpClient, userId);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
