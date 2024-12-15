using Business;
using ConsoleApp;
using ConsoleApp.Commands;
using DAL.Databases;
using System.Net.Http.Json;

class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:7249/");

        var invoker = new CommandInvoker();

        Console.Write("Please enter your user ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            bool exit = false;

            while (!exit)
            {
                ShowMenu();

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // List Available Events
                        Console.Clear();
                        Console.WriteLine("List Available Events");
                        await ExecuteCommandAsync(invoker, new ListEventsCommand(httpClient));
                        break;

                    case "2": // Purchase Event Tickets
                        Console.Clear();
                        Console.WriteLine("Purchase Event Tickets");
                        Console.Write("Enter the Event ID: ");
                        var eventId = Console.ReadLine();

                        Console.Write("Enter the number of tickets to purchase: ");
                        if (int.TryParse(Console.ReadLine(), out int ticketCount))
                        {
                            await ExecuteCommandAsync(invoker, new PurchaseTicketCommand(httpClient, userId, eventId, ticketCount));
                        }
                        else
                        {
                            ShowErrorMessage("Invalid number of tickets. Please enter a valid integer.");
                        }
                        break;

                    case "3": // View Purchased Tickets
                        Console.Clear();
                        Console.WriteLine("View Purchased Tickets");
                        await ExecuteCommandAsync(invoker, new ViewTicketsCommand(httpClient, userId));
                        break;

                    case "4": // Get Account Balance
                        Console.Clear();
                        Console.WriteLine("Get Account Balance");
                        await ExecuteCommandAsync(invoker, new GetUserBalanceCommand(httpClient, userId));
                        break;

                    case "5": // Add User Balance
                        Console.Clear();
                        Console.WriteLine("Add User Balance");
                        Console.Write("Enter the amount to add: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                        {
                            await ExecuteCommandAsync(invoker, new AddUserBalanceCommand(httpClient, userId, amount));
                        }
                        else
                        {
                            ShowErrorMessage("Invalid amount. Please enter a valid decimal number.");
                        }
                        break;

                    case "6": // Exit
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        Console.ResetColor();
                        exit = true;
                        break;

                    default:
                        ShowErrorMessage("Invalid choice. Please enter a valid option.");
                        break;
                }

            }
        }
        else
        {
            ShowErrorMessage("Invalid user ID. Please enter a valid integer.");
        }
    }

    static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. List Available Events");
        Console.WriteLine("2. Purchase Event Tickets");
        Console.WriteLine("3. View Purchased Tickets");
        Console.WriteLine("4. Get Account Balance");
        Console.WriteLine("5. Add User Balance");
        Console.WriteLine("6. Exit");
        Console.Write("Enter your choice (1-6): ");

    }

    static async Task ExecuteCommandAsync(CommandInvoker invoker, ICommand command)
    {
        invoker.ClearCommands();
        invoker.AddCommand(command);
        await invoker.ExecuteCommandsAsync();
        GoBackToMenu();
    }

    static void GoBackToMenu()
    {
        Console.WriteLine("\nPress any key to go back to the main menu...");
        Console.ReadKey();
    }

    static void ShowErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
