using ConsoleApp.Commands;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleApp.Tests.Commands
{
    public class CommandInvokerTests
    {
        [Fact]
        public async Task ExecuteCommandsAsync_ExecutesAllCommandsInOrder_WhenCommandsAreInList()
        {
            // Arrange
            var mockCommand1 = new Mock<ICommand>();
            var mockCommand2 = new Mock<ICommand>();

            var invoker = new CommandInvoker();
            invoker.AddCommand(mockCommand1.Object);
            invoker.AddCommand(mockCommand2.Object);

            // Act
            await invoker.ExecuteCommandsAsync();

            // Assert
            mockCommand1.Verify(command => command.ExecuteAsync(), Times.Once);
            mockCommand2.Verify(command => command.ExecuteAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteCommandsAsync_DoesNotThrowExceptions_WhenCommandListIsEmpty()
        {
            // Arrange
            var invoker = new CommandInvoker();

            // Act & Assert
            await invoker.ExecuteCommandsAsync(); // Should not throw any exceptions
        }

        [Fact]
        public void ClearCommands_RemovesAllCommands_WhenCalled()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var invoker = new CommandInvoker();
            invoker.AddCommand(mockCommand.Object);

            // Act
            invoker.ClearCommands();

            // Assert
            // No commands should be executed after clearing
            Assert.Empty(invoker.GetCommandsForTesting());
        }

        [Fact]
        public void AddCommand_AddsCommandToList_WhenValidCommandIsProvided()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var invoker = new CommandInvoker();

            // Act
            invoker.AddCommand(mockCommand.Object);

            // Assert
            Assert.Single(invoker.GetCommandsForTesting());
        }
    }

    // Extension method to expose private fields for testing (optional for test purposes only)
    public static class CommandInvokerExtensions
    {
        public static List<ICommand> GetCommandsForTesting(this CommandInvoker invoker)
        {
            var type = typeof(CommandInvoker);
            var field = type.GetField("_commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<ICommand>)field.GetValue(invoker);
        }
    }
}
