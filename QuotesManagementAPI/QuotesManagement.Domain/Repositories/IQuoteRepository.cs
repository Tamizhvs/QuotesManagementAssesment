using QuotesManagement.Domain.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesManagement.Domain.Repositories
{
    public interface IQuoteRepository
    {
        Task<IEnumerable<Quote>> GetAllQuotesAsync();
        Task<Quote> GetQuoteByIdAsync(int id);
        Task AddQuotesAsync(Quote quote);
        Task UpdateQuoteAsync(Quote quote);
        Task DeleteQuoteAsync(int id);
        Task<IEnumerable<Quote>> SearchQuotesAsync(string author, List<string> tags, string quote);
    }
}
