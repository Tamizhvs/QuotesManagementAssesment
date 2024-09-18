using QuotesManagement.Domain.DTO;
using QuotesManagement.Domain.Entity;
using QuotesManagement.Domain.Repositories;
using QuotesManagement.Domain.ResultSet;
using QuotesManagement.Domain.Services;

namespace QuotesManagement.Persistence.Services
{
    public class QuoteService : IQuoteService
    {

        private readonly IQuoteRepository _quoteRepository;

        public QuoteService(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        }

        public async Task<IEnumerable<QuoteResult>> GetAllQuotesAsync()
        {
            var quotes = await _quoteRepository.GetAllQuotesAsync();
            return quotes.Select(t => new QuoteResult
            {
                Id = t.Id,
                Author = t.Author,
                Tags = t.Tags.Split(',').ToList(),
                Quote = t.QuoteText
            });
        }

        public async Task<QuoteResult> GetQuoteByIdAsync(int id)
        {
            var quote = await _quoteRepository.GetQuoteByIdAsync(id);
            if (quote == null)
                return null;

            return new QuoteResult
            {
                Id = quote.Id,
                Author = quote.Author,
                Tags = quote.Tags.Split(',').ToList(),
                Quote = quote.QuoteText
            };
        }

        public async Task AddQuotesAsync(IEnumerable<CreateQuoteDTO> quotes)
        {
            foreach (var quote in quotes)
            {
                var quoteItem = new Quote
                {
                    Author = quote.Author,
                    Tags = string.Join(",", quote.Tags.Where(tag => !string.IsNullOrEmpty(tag))),
                    QuoteText = quote.Quote
                };
                await _quoteRepository.AddQuotesAsync(quoteItem);
            }
        }

        public async Task UpdateQuoteAsync(UpdateQuoteDTO quote)
        {
            var existingQuote = await _quoteRepository.GetQuoteByIdAsync(quote.Id);
            if (existingQuote == null)
                return;

            existingQuote.Author = quote.Author;
            existingQuote.Tags = string.Join(",", quote.Tags.Where(tag => !string.IsNullOrEmpty(tag)));

            existingQuote.QuoteText = quote.Quote;

            await _quoteRepository.UpdateQuoteAsync(existingQuote);
        }

        public async Task DeleteQuoteAsync(int id)
        {
            await _quoteRepository.DeleteQuoteAsync(id);
        }

        public async Task<IEnumerable<QuoteResult>> SearchQuotesAsync(string author, List<string> tags, string quoteContent)
        {
            var quotes = await _quoteRepository.SearchQuotesAsync(author, tags, quoteContent);
            return quotes.Select(t => new QuoteResult
            {
                Id = t.Id,
                Author = t.Author,
                Tags = t.Tags.Split(',').ToList(),
                Quote = t.QuoteText
            });
        }

    }
}
