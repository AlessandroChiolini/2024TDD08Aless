using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CopyTransaction
    {
        public CopyTransaction() { }

        public CopyTransaction(int id, int userId, int numberOfCopies, decimal amount, DateTime date)
        {
            Id = id; 
            UserId = userId;
            NumberOfCopies = numberOfCopies;
            Amount = amount;
            Date = date;
        }

        public CopyTransaction(int userId, int numberOfCopies, decimal amount, DateTime date)
        {
            UserId = userId;
            NumberOfCopies = numberOfCopies;
            Amount = amount;
            Date = date;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int NumberOfCopies { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}