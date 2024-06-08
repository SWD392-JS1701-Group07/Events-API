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
    }
}
