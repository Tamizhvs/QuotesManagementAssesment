using QuotesManagement.Domain.DTO;
using QuotesManagement.Domain.Entity;
using QuotesManagement.Domain.ResultSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesManagement.Domain.Services
{
    public interface IQuoteService
    {
        Task<IEnumerable<QuoteResult>> GetAllQuotesAsync();
        Task<QuoteResult> GetQuoteByIdAsync(int id);
        Task AddQuotesAsync(IEnumerable<CreateQuoteDTO> createQuoteDTOs);
        Task UpdateQuoteAsync(UpdateQuoteDTO updateQuoteDTO);
        Task DeleteQuoteAsync(int id);
        Task<IEnumerable<QuoteResult>> SearchQuotesAsync(string author, List<string> tags, string quoteContent);
        Task<List<string>> GetAllTagNamesAsync();
        Task<IEnumerable<Tags>> GetAllTagsAsync();
    }
}
