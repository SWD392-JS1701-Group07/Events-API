using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> CheckCustomerExist(string email, string phoneNumber);
        Task<bool> CreateCustomer(Customer customer);
        Task<Customer> GetCustomerByEmail(string email);
        Task<Customer> GetCustomerByPhoneNumber(string phoneNumber);
    }
}
