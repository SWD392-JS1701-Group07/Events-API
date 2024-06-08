using Events.Models.DTOs;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDTO> CheckLogin(string username, string password);
        Task<BaseResponse> GetAllAccounts();
        Task<BaseResponse> GetAccountById(int id);
    }
}
