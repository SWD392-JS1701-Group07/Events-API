using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
	public interface IOrderService
	{
		Task<bool> UpdateOrderStatus(string orderId, string? responeCode);
	}
}
