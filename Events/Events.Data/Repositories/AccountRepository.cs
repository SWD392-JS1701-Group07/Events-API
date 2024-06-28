using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Utils;

namespace Events.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EventsDbContext _context;
        public AccountRepository(EventsDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountByUsername(string username)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Username == username);
        }
        public async Task<Account> GetAccountByPhoneNumber(string phoneNumber)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber);
        }
        public async Task<Account> GetAccountByEmail(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<bool> CreateAccount(Account account)
        {
            _context.Accounts.AddAsync(account);
            return await _context.SaveChangesAsync() > 0;   
        }

        public async Task<Account> GetAccount(string username, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Username == username && e.Password == password);
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<bool> BanAccount(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(e => e.Id == id);

            if(account == null)
            {
                return false;
            }
            else
            {
                try
                {
                    account.AccountStatus = Enums.AccountStatus.Banned;
                    _context.Entry(account).State = EntityState.Modified;

                    return await _context.SaveChangesAsync() > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<bool> UpdateAccount(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProfile(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Account>> GetAccountByRole(int roleId)
        {
            return await _context.Accounts.Where(e => e.RoleId == roleId).ToListAsync();
        }

        public async
            
            
            
            Task<bool> RegisterAccount(Account account)
        {
            _context.Accounts.AddAsync(account);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
