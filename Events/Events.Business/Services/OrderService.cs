using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Events.Utils;
using Events.Utils.Helpers;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Events.Utils.Enums;

namespace Events.Business.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly ITicketService _ticketService;
		private readonly IEventService _eventService;
		private readonly IVNPayPaymentService _vnPayPaymentService;
		private readonly IMapper _mapper;
		private readonly EventsDbContext _dbContext;
		private readonly ITransactionRepository _transactionRepository;
		private readonly EmailHelper _emailHelper;

		public OrderService(IOrderRepository orderRepository, ITicketRepository ticketRepository, ITicketService ticketService, IEventService eventService, IVNPayPaymentService vnPayPaymentService, IMapper mapper, EventsDbContext dbContext, ITransactionRepository transactionRepository, EmailHelper emailHelper)
		{
			_orderRepository=orderRepository;
			_ticketRepository=ticketRepository;
			_ticketService=ticketService;
			_eventService=eventService;
			_vnPayPaymentService=vnPayPaymentService;
			_mapper=mapper;
			_dbContext=dbContext;
			_transactionRepository=transactionRepository;
			_emailHelper=emailHelper;
		}

		public async Task<BaseResponse> CreateOrderAndPayment(CreateOrderRequest request, HttpContext context)
		{
			try
			{
				//check ticket exist
				var validationResponse = await ValidateRequest(request);
				if (!validationResponse.IsSuccess)
				{
					return validationResponse;
				}
				//Get total price from db
				var totalPriceFromDb = await _eventService.GetTotalPriceTicketOfEvent(request.Tickets);
				if (request.TotalAmount != totalPriceFromDb)
				{
					return new BaseResponse
					{
						StatusCode = StatusCodes.Status400BadRequest,
						Message = "Total Amount is not correct!",
						IsSuccess = false
					};
				}
				using (var transaction = _dbContext.Database.BeginTransaction())
				{
					try
					{
						var orderEntity = _mapper.Map<Order>(request);
						orderEntity.Id = Guid.NewGuid().ToString();
						orderEntity.OrderDate = DateTime.Now;
						orderEntity.OrderStatus = request.TotalAmount == 0 ? OrderStatus.Success : OrderStatus.Pending;
						await _orderRepository.CreateOrders(orderEntity);
						// Loop for list of ticket in request
						foreach (var ticketDetail in request.Tickets)
						{
							var ticketEntity = _mapper.Map<Ticket>(ticketDetail);
							ticketEntity.Id = Guid.NewGuid().ToString();
							ticketEntity.IsCheckIn = IsCheckin.No;
							ticketEntity.OrdersId = orderEntity.Id;
							var ticketInfo = _mapper.Map<QrCodeDTO>(ticketEntity);
							ticketInfo.Price = ticketEntity.Price;
							ticketInfo.EventName = await _eventService.GetEventNameByIdAsync(ticketDetail.EventId);
							ticketEntity.Qrcode = QrHelper.GenerateQr(JsonSerializer.Serialize(ticketInfo));
							await _ticketRepository.CreateTicket(ticketEntity);
						}

						// Create transaction record
						if (request.TotalAmount > 0)
						{
							var transactionEntry = new Transaction
							{
								Id = Guid.NewGuid().ToString(),
								OrderId = orderEntity.Id,
								Amount = request.TotalAmount,
								PaymentStatus = PaymentStatus.Pending, // Pending
							};
							await _transactionRepository.AddTransaction(transactionEntry);
						}

						await _dbContext.SaveChangesAsync();
						await transaction.CommitAsync();

						if (request.TotalAmount > 0)
						{
							var payUrl = _vnPayPaymentService.CreatePayment(request.TotalAmount.ToString(), $"Payment for order #{orderEntity.Id}", context);
							return new BaseResponse { IsSuccess = true, Message = "Order created. Proceed to payment.", Data = payUrl };
						}

						var orderFromDb = await _orderRepository.GetOrderByIdAsync(orderEntity.Id);
						var listTicket = orderFromDb.Tickets.ToList();
						if (listTicket != null)
						{
							foreach(var ticket in listTicket)
							{
								_emailHelper.SendEmailToBuyTicketSuccess(ticket.Email, ticket.Qrcode);
							}
						}
						return new BaseResponse { StatusCode = StatusCodes.Status200OK , IsSuccess = true, Message = "Order created successfully. No payment needed!!" };

					}
					catch (Exception)
					{
						await transaction.RollbackAsync();
						throw;
					}
				}

			}
			catch (Exception ex)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = ex.Message,
					IsSuccess = false
				};
			}
		}

		public async Task<BaseResponse> HandlePaymentCallback(IQueryCollection query)
		{
			try
			{
				var response = _vnPayPaymentService.PaymentExecute(query);
				if (response.Success)
				{
					string orderId = "";
					string[] parts = response.OrderDescription.Split('#');
					if (parts.Length > 1)
					{
						orderId = parts[1].Trim();
					}
					var orderFromDb = await _orderRepository.GetOrderByIdAsync(orderId);
					var transaction = await _transactionRepository.GetTransactionFilter(t => t.OrderId == orderId && t.PaymentStatus == PaymentStatus.Pending,
																									t => t.TransactionDate);
					if (orderFromDb != null)
					{
						if (orderFromDb.OrderStatus == OrderStatus.Pending)
						{
							if (response.VnPayResponseCode == "00")
							{
								using (var transactionDb = _dbContext.Database.BeginTransaction())
									try
									{
										{
											if (transaction != null)
											{
												_mapper.Map(response, transaction);
												transaction.ResponseMessage = "Payment success!";
												await _transactionRepository.UpdateTransactionAsync(transaction);
											}

											orderFromDb.OrderStatus = OrderStatus.Success;
											await _orderRepository.UpdateOrderStatusAsync(orderFromDb);

											var ticketBought = await _ticketService.GetTicketFilter(isBought: true, orderId: orderFromDb.Id, includeProps: "Orders");
											if (ticketBought.Any())
											{
												var eventTicketQuantities = ticketBought
														.GroupBy(ticket => ticket.EventId)
														.ToDictionary(group => group.Key, group => group.Count());
										
												bool isSuccess = await _eventService.UpdateTicketQuantity(eventTicketQuantities);
												if (!isSuccess)
												{
													return new BaseResponse
													{
														StatusCode = StatusCodes.Status500InternalServerError,
														Message = "There was an error when updating ticket quantities!!",
														IsSuccess = false
													};
												}
											}
											await transactionDb.CommitAsync();

											//Send Email
											var listTicket = orderFromDb.Tickets.ToList();
											if (listTicket != null)
											{
												foreach (var ticket in listTicket)
												{
													_emailHelper.SendEmailToBuyTicketSuccess(ticket.Email, ticket.Qrcode);
												}
											}

											return new BaseResponse
											{
												StatusCode = StatusCodes.Status200OK,
												Message = "Payment callback handled successfully!!",
												IsSuccess = true,
												Data = response
											};
										}
									}
									catch (Exception)
									{
										await transactionDb.RollbackAsync();
										throw;
									}

							}
							else
							{
								using(var transactionDb = _dbContext.Database.BeginTransaction())
								{
									try
									{
										if (transaction != null)
										{
											_mapper.Map(response, transaction);
											transaction.ResponseMessage = "Payment failed!";
											await _transactionRepository.UpdateTransactionAsync(transaction);
										}
										orderFromDb.OrderStatus = OrderStatus.Failed;
										await _orderRepository.UpdateOrderStatusAsync(orderFromDb);
										return new BaseResponse
										{
											StatusCode = StatusCodes.Status400BadRequest,
											Message = "Payment failed",
											IsSuccess = false,
											Data = response
										};
									}
									catch (Exception) { 
										await transactionDb.RollbackAsync();
										throw;
									}
								}
							}
						}
						else
						{
							return new BaseResponse
							{
								StatusCode = StatusCodes.Status400BadRequest,
								Message = "Order status is not pending",
								IsSuccess = false
							};
						}
					}
					else
					{
						return new BaseResponse
						{
							StatusCode = StatusCodes.Status404NotFound,
							Message = "Order not found",
							IsSuccess = false
						};
					}
				}
				else
				{
					return new BaseResponse
					{
						StatusCode = StatusCodes.Status400BadRequest,
						Message = "Payment execution failed",
						IsSuccess = false,
						Data = response
					};
				}
			}
			catch (KeyNotFoundException e)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = e.Message,
					IsSuccess = false
				};
			}
			catch (Exception e)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = e.Message,
					IsSuccess = false
				};
			}
		}

		private async Task<BaseResponse> ValidateRequest(CreateOrderRequest request)
		{
			var errors = new Dictionary<string, string>();
			foreach (var ticketDetail in request.Tickets)
			{
				var ticketExists = await _ticketRepository.CheckTicketExist(ticketDetail.Email, ticketDetail.PhoneNumber, ticketDetail.EventId);
				if (ticketExists)
				{
					var key = $"{ticketDetail.Email}-{ticketDetail.PhoneNumber}";
					if (!errors.ContainsKey(key))
					{
						errors.Add(key, $"A ticket with phone number '{ticketDetail.PhoneNumber}' and email '{ticketDetail.Email}' already exists for event '{ticketDetail.EventId}'");
					}
				}
				var eventResponse = await _eventService.GetEventById(ticketDetail.EventId);
				if(!eventResponse.IsSuccess)
				{
					var key = $"{ticketDetail.EventId}";
					if(!errors.ContainsKey(key))
					{
						errors.Add(key, $"Not found Event with event id '{ticketDetail.EventId}'");
					}
				}
			}
			if (errors.Count!=0)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = "There are some errors in the request!!",
					IsSuccess = false,
					Errors = errors
				};
			}
			return new BaseResponse { IsSuccess = true };
		}
	}
}

