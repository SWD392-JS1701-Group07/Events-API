using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
	public interface ITransactionRepository
	{
		Task AddTransaction(Transaction transaction);
		Task<Transaction> GetTransactionFilter(Expression<Func<Transaction, bool>> filters, Expression<Func<Transaction, object>> filters2);
		Task UpdateTransactionAsync(Transaction transaction);
	}
}
