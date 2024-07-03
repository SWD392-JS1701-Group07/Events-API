using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetAllCustomersAsync();
        Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDTO updateCustomerDto);
        Task<CustomerDTO> GetCustomerByIdAsync(int id);
        Task<bool> DeleteCustomerAsync(int id);
        Task<CustomerDTO> CreateCustomerAsync(UpdateCustomerDTO createCustomerDto);
    }
}
