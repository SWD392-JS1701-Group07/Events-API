using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Http;
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
        Task<BaseResponse> GetAccountByRole(int roleId);
        Task<BaseResponse> CreateAccount(CreateAccountDTO createAccountDTO);
        Task<BaseResponse> RegisterAccount(RegisterAccountDTO registerAccountDTO);
        Task<BaseResponse> BanAccount(int id);
        Task<BaseResponse> UpdateAccount(int id, UpdateAccountDTO updateAccountDTO);
        Task<BaseResponse> UpdateProfile(int id, UpdateProfile updateProfile, IFormFile avatarFile);
    }
}
