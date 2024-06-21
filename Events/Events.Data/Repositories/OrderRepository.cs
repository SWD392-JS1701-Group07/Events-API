using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EventsDbContext _context;

        public OrderRepository(EventsDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateOrders(Order order)
        {
            var result = await _context.Orders.AddAsync(order);
            return result != null;
        }
    }
}
