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
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Get User Copy Transactions");
                        await ExecuteCommandAsync(invoker, new GetUserCopyTransactionsCommand(httpClient, userId));
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("Process Copy Payment");
                        Console.Write("Enter the number of copies (0.20$ per copy): ");
                        if (int.TryParse(Console.ReadLine(), out int numberOfCopies))
                        {
                            await ExecuteCommandAsync(invoker, new ProcessCopyPaymentCommand(httpClient, userId, numberOfCopies));
                        }
                        else
                        {
                            ShowErrorMessage("Invalid number of copies. Please enter a valid integer.");
                        }
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("Current Account Balance");
                        await ExecuteCommandAsync(invoker, new GetUserBalanceCommand(httpClient, userId));
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("Add User Balance");
                        Console.Write("Enter the amount to add: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal addAmount))
                        {
                            await ExecuteCommandAsync(invoker, new AddUserBalanceCommand(httpClient, userId, addAmount));
                        }
                        else
                        {
                            ShowErrorMessage("Invalid amount. Please enter a valid decimal number.");
                        }
                        break;

                    case "5":
                        exit = true;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        Console.ResetColor();
                        break;

                    default:
                        ShowErrorMessage("Invalid choice. Please enter 1, 2, 3, 4, or 5.");
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
        Console.WriteLine("1. Get User Copy Transactions");
        Console.WriteLine("2. Process Copy Payment");
        Console.WriteLine("3. Get Account Balance");
        Console.WriteLine("4. Add User Balance");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice (1, 2, 3, 4, or 5): ");
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
