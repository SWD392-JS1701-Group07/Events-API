using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Events.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<List<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public async Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDTO updateCustomerDto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            // Validate that no properties contain only whitespace
            ValidationUtils.ValidateNoWhitespaceOnly(updateCustomerDto);

            // Check if email is in a valid format
            if (!ValidationUtils.IsValidEmail(updateCustomerDto.Email))
            {
                throw new Exception("Invalid email format.");
            }

            // Check if phone number contains only digits
            if (!ValidationUtils.IsPhoneNumber(updateCustomerDto.PhoneNumber))
            {
                throw new Exception("Phone number should contain only digits.");
            }

            var existingCustomerByEmail = await _customerRepository.GetCustomerByEmail(updateCustomerDto.Email);
            if (existingCustomerByEmail != null && existingCustomerByEmail.Id != id)
            {
                throw new Exception("Email is already in use.");
            }

            var existingCustomerByPhone = await _customerRepository.GetCustomerByPhoneNumber(updateCustomerDto.PhoneNumber);
            if (existingCustomerByPhone != null && existingCustomerByPhone.Id != id)
            {
                throw new Exception("Phone number is already in use.");
            }

            _mapper.Map(updateCustomerDto, customer);
            return await _customerRepository.UpdateCustomerAsync(customer);
        }
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            return await _customerRepository.DeleteCustomerAsync(customer);
        }
        public async Task<CustomerDTO> CreateCustomerAsync(UpdateCustomerDTO createCustomerDto)
        {
            // Validate that no properties contain only whitespace
            ValidationUtils.ValidateNoWhitespaceOnly(createCustomerDto);

            // Check if email is in a valid format
            if (!ValidationUtils.IsValidEmail(createCustomerDto.Email))
            {
                throw new Exception("Invalid email format.");
            }

            // Check if phone number contains only digits
            if (!ValidationUtils.IsPhoneNumber(createCustomerDto.PhoneNumber))
            {
                throw new Exception("Phone number should contain only digits.");
            }

            var customerExists = await _customerRepository.CheckCustomerExist(createCustomerDto.Email, createCustomerDto.PhoneNumber);
            if (customerExists)
            {
                throw new Exception("Email or phone number is already in use.");
            }

            var customer = _mapper.Map<Customer>(createCustomerDto);
            var createResult = await _customerRepository.CreateCustomer(customer);

            if (!createResult)
            {
                throw new Exception("Failed to create customer.");
            }

            return _mapper.Map<CustomerDTO>(customer);
        }
    }
}
