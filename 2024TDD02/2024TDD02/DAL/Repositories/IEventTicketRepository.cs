using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IEventTicketRepository
    {
        int GetNextId();
        void AddTicket(EventTicket ticket);
        List<EventTicket> GetTicketsByUserId(int userId);
    }


}
