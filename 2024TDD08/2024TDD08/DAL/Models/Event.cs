﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int AvailableTickets { get; set; }
        public decimal TicketPrice { get; set; }

        public bool HasAvailableTickets(int quantity) => AvailableTickets >= quantity;

        public bool ReserveTickets(int quantity)
        {
            if (AvailableTickets >= quantity)
            {
                AvailableTickets -= quantity;
                return true;
            }
            return false;
        }
    }

}