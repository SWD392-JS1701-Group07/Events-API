using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
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
            var existingCustomerByEmail = await _customerRepository.GetCustomerByEmail(createCustomerDto.Email);
            if (existingCustomerByEmail != null)
            {
                throw new Exception("Email is already in use.");
            }

            var existingCustomerByPhone = await _customerRepository.GetCustomerByPhoneNumber(createCustomerDto.PhoneNumber);
            if (existingCustomerByPhone != null)
            {
                throw new Exception("Phone number is already in use.");
            }

            var customer = _mapper.Map<Customer>(createCustomerDto);
            await _customerRepository.CreateCustomer(customer);

            return _mapper.Map<CustomerDTO>(customer);
        }
    }
}
