using Microsoft.EntityFrameworkCore;
using QuotesManagement.Domain.Entity;
using QuotesManagement.Domain.Repositories;
using QuotesManagement.Persistence.Data;

namespace QuotesManagement.Persistence.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly QuoteContext _context;

        public QuoteRepository(QuoteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
        {

            return await _context.Quotes.ToListAsync();

        }

        public async Task<Quote> GetQuoteByIdAsync(int id)
        {

            return await _context.Quotes.FindAsync(id);
        }

        public async Task AddQuotesAsync(Quote quote)
        {
            await _context.Quotes.AddAsync(quote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuoteAsync(Quote quote)
        {
            _context.Quotes.Update(quote);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuoteAsync(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote != null)
            {
                _context.Quotes.Remove(quote);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Quote>> SearchQuotesAsync(string author, List<string> tags, string quote)
        {
            var query = _context.Quotes.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(t => t.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(quote))
            {
                query = query.Where(t => t.QuoteText.ToLower().Contains(quote.ToLower()));
            }

            var result = await query.ToListAsync();


            if (tags != null && tags.Count > 0)
            {
                var lowerTags = tags
                               .Where(tag => !string.IsNullOrEmpty(tag))
                               .Select(tag => tag.ToLower().Trim())
                               .ToList();

                if (lowerTags.Any(tag => tag.Contains(",")))
                {
                    lowerTags = lowerTags.SelectMany(tag => tag.Split(',')).ToList();
                }
                result = result.Where(t => t.Tags != null && t.Tags
                    .Split(',')
                    .Select(tag => tag.Trim().ToLower())
                    .Any(tag => lowerTags.Contains(tag)))
                    .ToList();
            }

            return result;
        }
    }
}
