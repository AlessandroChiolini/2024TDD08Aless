using Business;
using ConsoleApp;
using ConsoleApp.Commands;
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
            // Fetch user information
            var user = await FetchUserByIdAsync(httpClient, userId);
            if (user == null)
            {
                ShowErrorMessage("User not found. Please check the ID and try again.");
                return;
            }

            bool exit = false;

            while (!exit)
            {
                ShowMenuWithWelcome(user.Name); // Show menu with the welcome message

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
                            var response = await httpClient.PostAsJsonAsync(
                                $"api/User/{userId}/balance/add", new { Amount = amount });

                            if (response.IsSuccessStatusCode)
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Balance updated successfully!");
                                Console.WriteLine(result);
                            }
                            else
                            {
                                var error = await response.Content.ReadAsStringAsync();
                                ShowErrorMessage($"Failed to add balance: {error}");
                            }
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

    static async Task<User> FetchUserByIdAsync(HttpClient httpClient, int userId)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/User/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<User>();
                return user;
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error fetching user details: {ex.Message}");
        }
        return null;
    }

    static void ShowMenuWithWelcome(string userName)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Welcome, {userName}!\n");
        Console.ResetColor();

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

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
}
