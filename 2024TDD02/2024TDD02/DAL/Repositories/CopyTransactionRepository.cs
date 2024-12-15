using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Databases;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface ICopyTransactionRepository<CopyTransaction>
    {
        void Add(Models.CopyTransaction copyTransaction);
        List<CopyTransaction> GetUserCopyTransactions(int userId);
        void SaveChanges();
    }
    public class CopyTransactionRepository : ICopyTransactionRepository<CopyTransaction>
    {
        private readonly AppDbContext _context;

        public CopyTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<CopyTransaction> GetUserCopyTransactions(int userId)
        {
            return _context.CopyTransactions
                .Where(x => x.UserId == userId)
                .ToList();
        }
        public void Add(CopyTransaction copyTransaction)
        {
            _context.CopyTransactions.Add(copyTransaction);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}