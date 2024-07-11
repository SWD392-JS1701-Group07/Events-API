using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Utils;
using System.Linq.Expressions;

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

        public async Task<Account> CreateAccount(Account account)
        {
            await _context.Accounts.AddAsync(account);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return account;
            }
            else
            {
                return null;
            }
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
            return await _context.Accounts.Where(e => e.AccountStatus == Enums.AccountStatus.Active).ToListAsync();
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

        public async Task<List<Account>> GetAccountByRole(int roleId, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Account> query = _context.Accounts.Where(e => e.RoleId == roleId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortOrder))
            {
                Expression<Func<Account, object>> keySelector = sortColumn switch
                {
                    "name" => e => e.Name,
                    "email" => e => e.Email,
                    _ => e => e.Id,
                };

                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(keySelector),
                    "desc" => query.OrderByDescending(keySelector),
                    _ => query.OrderBy(keySelector)
                };
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var events = await query.ToListAsync();
            return events;
        }
        public async Task<bool> RegisterAccount(Account account)
        {
            _context.Accounts.AddAsync(account);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Account> GetAccountByStudentId(string studentId)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.StudentId == studentId);
        }
    }
}
