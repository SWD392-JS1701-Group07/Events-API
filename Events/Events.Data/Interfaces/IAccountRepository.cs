using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccount(string username, string password);
    }
}
