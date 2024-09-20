using QuotesManagementUI.DTO;
using QuotesManagementUI.ResultSet;

namespace QuotesManagementUI.QuotesServices
{
    public interface IQuotesApiService
    {
        Task<IEnumerable<QuoteResult>> GetAllQuotes();
        Task<QuoteResult> GetQuoteById(int id);
        Task AddQuotes(IEnumerable<CreateQuoteDTO> quote);
        Task UpdateQuote(UpdateQuoteDTO quote);
        Task DeleteQuote(int id);
        Task<IEnumerable<QuoteResult>> SearchQuotes(string author, List<string> tags, string quoteContent);
        Task<List<string>> GetAllTagNames();
    }
}
