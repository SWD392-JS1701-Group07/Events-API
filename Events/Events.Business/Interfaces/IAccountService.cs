using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Interfaces
{
    public interface IAccountService
    {
        Task<Account> CheckLogin(string username, string password);
    }
}
