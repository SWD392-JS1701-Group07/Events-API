using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EventsDbContext _context;
        public AccountRepository(EventsDbContext context)
        {
            _context = context;
        }
        public async Task<Account> GetAccount(string username, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Username == username && e.Password == password);
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }
    }
}
