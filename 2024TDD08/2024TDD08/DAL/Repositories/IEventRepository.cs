﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IEventRepository
    {
        Event GetEventById(string eventId);
        List<Event> GetAvailableEvents();
        void UpdateEvent(Event eventItem);
    }

}
