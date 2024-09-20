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
                Tags = t.QuoteTags.Select(qt => qt.Tags.TagName).ToList(),
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
                Tags = quote.QuoteTags.Select(qt => qt.Tags.TagName).ToList(),
                Quote = quote.QuoteText
            };
        }

        public async Task AddQuotesAsync(IEnumerable<CreateQuoteDTO> quotes)
        {
            foreach (var quoteDto in quotes)
            {
                var quote = new Quote
                {
                    Author = quoteDto.Author,
                    QuoteText = quoteDto.Quote,
                    QuoteTags = new List<QuoteTags>()
                };
                await ProcessTagsForQuoteAsync(quote, quoteDto.Tags);
                await _quoteRepository.AddQuotesAsync(quote);
            }
        }
        public async Task UpdateQuoteAsync(UpdateQuoteDTO quote)
        {
            var existingQuote = await _quoteRepository.GetQuoteByIdAsync(quote.Id);
            if (existingQuote == null)
                return;

            existingQuote.Author = quote.Author;
            existingQuote.QuoteText = quote.Quote;

            await ProcessTagsForQuoteAsync(existingQuote, quote.Tags);
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
                Tags = t.QuoteTags.Select(qt => qt.Tags.TagName).ToList(),
                Quote = t.QuoteText
            });
        }

        public async Task<List<string>> GetAllTagNamesAsync()
        {
            var tags = await _quoteRepository.GetAllTagNamesAsync();
            return tags;
        }

        private async Task ProcessTagsForQuoteAsync(Quote quote, List<string> tagNames)
        {
            var distinctTags = tagNames.Where(tag => !string.IsNullOrWhiteSpace(tag)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var existingTags = await _quoteRepository.GetAllTagsAsync();
            quote.QuoteTags.Clear();
            foreach (var tagName in distinctTags)
            {
                var existingTag = existingTags.FirstOrDefault(t => string.Equals(t.TagName, tagName, StringComparison.OrdinalIgnoreCase));
                if (existingTag == null)
                {
                    var newTag = await _quoteRepository.AddTagAsync(tagName);
                    quote.QuoteTags.Add(new QuoteTags { TagId = newTag.TagId });
                }
                else
                {
                    quote.QuoteTags.Add(new QuoteTags { TagId = existingTag.TagId });
                }
            }
        }

        public async Task<IEnumerable<Tags>> GetAllTagsAsync()
        {
            var tags = await _quoteRepository.GetAllTagsAsync();
            return tags;
        }

    }
}
