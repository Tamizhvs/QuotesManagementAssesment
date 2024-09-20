using QuotesManagement.Domain.Entity;

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
        Task<List<string>> GetAllTagNamesAsync();
        Task<Tags> AddTagAsync(string tagName);
        Task<IEnumerable<Tags>> GetAllTagsAsync();
    }
}
