﻿using Events.Models.DTOs.Request;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> CreateOrders(Order order);
    }
}
