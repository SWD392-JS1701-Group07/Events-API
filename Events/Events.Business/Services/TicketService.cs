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
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<BaseResponse> CreateFreeTicket(CreateTicketRequestDTO createTicketRequestDTO)
        {
            var eventExist = await _eventRepository.GetEventByIdAsync(createTicketRequestDTO.EventId);  
            if(eventExist == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can't found this event"
                };
            }
            else
            {
                var ticketExist = await _ticketRepository.CheckTicketExist(createTicketRequestDTO.Email, createTicketRequestDTO.PhoneNumber, createTicketRequestDTO.EventId);
                if(ticketExist)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "This email/phone has bought this ticket before"
                    };
                }
                else
                {
                    var cusExist = await _customerRepository.CheckCustomerExist(createTicketRequestDTO.Email, createTicketRequestDTO.PhoneNumber);
                    if(!cusExist)
                    {
                        var emailExist = await _customerRepository.GetCustomerByEmail(createTicketRequestDTO.Email);
                        var phoneNumberExist = await _customerRepository.GetCustomerByPhoneNumber(createTicketRequestDTO.PhoneNumber);
                        if(emailExist != null || phoneNumberExist != null)
                        {
                            return new BaseResponse
                            {
                                StatusCode = 500,
                                Data = null,
                                IsSuccess = false,
                                Message = "This customer existed before"
                            };
                        }
                        else
                        {
                            CustomerDTO customerCreate = new CustomerDTO
                            {
                                Name = createTicketRequestDTO.Name,
                                PhoneNumber = createTicketRequestDTO.PhoneNumber,
                                Email = createTicketRequestDTO.Email
                            };

                            var customerMapping = _mapper.Map<Customer>(customerCreate);

                            var cusCreate = await _customerRepository.CreateCustomer(customerMapping);



                            OrderDTO orderDTO = new OrderDTO
                            {
                                OrderDate = DateTime.UtcNow.AddHours(7),
                                TotalPrice = 0,
                                PaymentMethod = "Free",
                                OrderStatus = "Paid",
                                Email = createTicketRequestDTO.Email,
                                PhoneNumber = createTicketRequestDTO.PhoneNumber,
                                CustomerId = customerMapping.Id,
                            };


                            var orders = _mapper.Map<Order>(orderDTO);

                            var orderCreate = await _orderRepository.CreateOrders(orders);

                            TicketDTO ticket = new TicketDTO
                            {
                                Name = createTicketRequestDTO.Name,
                                PhoneNumber = createTicketRequestDTO.PhoneNumber,
                                Email = createTicketRequestDTO.Email,
                                EventId = createTicketRequestDTO.EventId,
                                Price = 0,
                                OrdersId = orders.Id
                            };

                            var ticketMapping = _mapper.Map<Ticket>(ticket);


                            var result = await _ticketRepository.CreateTicket(ticketMapping);

                            if (result && orderCreate && cusCreate)
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 200,
                                    Data = null,
                                    IsSuccess = false,
                                    Message = "Create successfully"
                                };
                            }
                            else
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 500,
                                    Data = null,
                                    IsSuccess = false,
                                    Message = "Create unsuccessful"
                                };
                            }

                        }
                    }
                    else
                    {
                        var cus = _customerRepository.GetCustomerByEmail(createTicketRequestDTO.Email);



                        OrderDTO orderDTO = new OrderDTO
                        {
                            OrderDate = DateTime.UtcNow.AddHours(7),
                            TotalPrice = 0,
                            PaymentMethod = "Free",
                            OrderStatus = "Paid",
                            Email = createTicketRequestDTO.Email,
                            PhoneNumber = createTicketRequestDTO.PhoneNumber,
                            CustomerId = cus.Id
                        };

                        var orders = _mapper.Map<Order>(orderDTO);

                        var orderCreate = await _orderRepository.CreateOrders(orders);

                        TicketDTO ticket = new TicketDTO
                        {
                            Name = createTicketRequestDTO.Name,
                            PhoneNumber = createTicketRequestDTO.PhoneNumber,
                            Email = createTicketRequestDTO.Email,
                            EventId = createTicketRequestDTO.EventId,
                            Price = 0,
                            OrdersId = orders.Id, 
                        };

                        var ticketMapping = _mapper.Map<Ticket>(ticket);

                        var result = await _ticketRepository.CreateTicket(ticketMapping);

                        if (result && orderCreate)
                        {
                            return new BaseResponse
                            {
                                StatusCode = 200,
                                Data = null,
                                IsSuccess = false,
                                Message = "Create successfully"
                            };
                        }
                        else
                        {
                            return new BaseResponse
                            {
                                StatusCode = 500,
                                Data = null,
                                IsSuccess = false,
                                Message = "Create unsuccessful"
                            };
                        }
                    }
                }
            }
        }
    }
}
