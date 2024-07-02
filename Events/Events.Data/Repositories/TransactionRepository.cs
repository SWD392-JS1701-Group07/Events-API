using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly EventsDbContext _context;

		public TransactionRepository(EventsDbContext context)
		{
			_context=context;
		}

		public async Task AddTransaction(Transaction transaction)
		{
			await _context.AddAsync(transaction);
		}

	

		public async Task<Transaction> GetTransactionFilter(Expression<Func<Transaction, bool>> filters, Expression<Func<Transaction, object>> filters2)
		{
			return await _context.Transactions.Where(filters).OrderByDescending(filters2).FirstOrDefaultAsync();
		}

		public async Task UpdateTransactionAsync(Transaction transaction)
		{
			_context.Entry(transaction).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}
	}
}
