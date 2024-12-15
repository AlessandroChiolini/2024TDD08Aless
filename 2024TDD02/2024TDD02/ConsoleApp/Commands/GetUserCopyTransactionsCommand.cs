using ConsoleApp.Commands;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class GetUserCopyTransactionsCommand : ICommand
{
    private readonly HttpClient _httpClient;
    private readonly int _userId;

    public GetUserCopyTransactionsCommand(HttpClient httpClient, int userId)
    {
        _httpClient = httpClient;
        _userId = userId;
    }

    public async Task ExecuteAsync()
    {
        var transactions = await GetUserCopyTransactionsAsync(_httpClient, _userId);

        if (transactions != null && transactions.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            // Display the transactions
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.Id}, User ID: {transaction.UserId}, Copies: {transaction.NumberOfCopies}, Amount: {transaction.Amount}, Date: {transaction.Date}");
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No transactions found for the given user ID.");
            Console.ResetColor();
        }
    }

    private static async Task<List<CopyTransaction>> GetUserCopyTransactionsAsync(HttpClient httpClient, int userId)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/CopyPayment/GetUserCopyTransactions?userId={userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CopyTransaction>>();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }
}
