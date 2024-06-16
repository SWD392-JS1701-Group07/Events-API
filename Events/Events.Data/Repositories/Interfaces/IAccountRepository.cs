using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccount(string username, string password);
        Task<List<Account>> GetAllAccounts();
        Task<Account> GetAccountById(int id);
        Task<List<Account>> GetAccountByRole(int roleId);
        Task<bool> CreateAccount(Account account);
        Task<bool> RegisterAccount(Account account);
        Task<bool> BanAccount(int id);
        Task<bool> UpdateAccount(Account account);
        Task<bool> UpdateProfile(Account account);
        Task<Account> GetAccountByUsername(string username);
        Task<Account> GetAccountByPhoneNumber(string phoneNumber);
        Task<Account> GetAccountByEmail(string email);
    }
}
