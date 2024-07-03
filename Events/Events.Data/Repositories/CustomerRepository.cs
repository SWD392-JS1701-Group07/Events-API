using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EventsDbContext _context;

        public CustomerRepository(EventsDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckCustomerExist(string email, string phoneNumber)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Email == email && e.PhoneNumber == phoneNumber);
            return customer != null;
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            var result = await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return result != null;
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteCustomerAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
