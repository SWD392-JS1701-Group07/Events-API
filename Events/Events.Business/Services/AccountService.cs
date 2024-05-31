using Events.Business.Interfaces;
using Events.Data.Interfaces;
using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;        
        }
        public async Task<Account> CheckLogin(string username, string password)
        {
            return await _accountRepository.GetAccount(username, password);
        }
    }
}
