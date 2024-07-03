using Events.Business.Services.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/customers")]
    [ApiVersionNeutral]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var response = new BaseResponse
            {
                StatusCode = 200,
                Message = "Customers retrieved successfully.",
                IsSuccess = true,
                Data = customers
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDTO updateCustomerDto)
        {
            var result = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
            if (!result)
            {
                var response = new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Customer not found.",
                    IsSuccess = false
                };
                return NotFound(response);
            }

            var updatedCustomer = await _customerService.GetCustomerByIdAsync(id);
            var successResponse = new BaseResponse
            {
                StatusCode = 200,
                Message = "Customer updated successfully.",
                IsSuccess = true,
                Data = updatedCustomer
            };
            return Ok(successResponse);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
            {
                var response = new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Customer not found.",
                    IsSuccess = false
                };
                return NotFound(response);
            }

            var successResponse = new BaseResponse
            {
                StatusCode = 200,
                Message = "Customer deleted successfully.",
                IsSuccess = true
            };
            return Ok(successResponse);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] UpdateCustomerDTO createCustomerDto)
        {
            try
            {
                var newCustomer = await _customerService.CreateCustomerAsync(createCustomerDto);
                var response = new BaseResponse
                {
                    StatusCode = 201,
                    Message = "Customer created successfully.",
                    IsSuccess = true,
                    Data = newCustomer
                };
                return CreatedAtAction(nameof(GetCustomerById), new { id = ((CustomerDTO)response.Data).Id }, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new BaseResponse
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    IsSuccess = false
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                var response = new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Customer not found.",
                    IsSuccess = false
                };
                return NotFound(response);
            }

            var successResponse = new BaseResponse
            {
                StatusCode = 200,
                Message = "Customer retrieved successfully.",
                IsSuccess = true,
                Data = customer
            };
            return Ok(successResponse);
        }
    }
}

