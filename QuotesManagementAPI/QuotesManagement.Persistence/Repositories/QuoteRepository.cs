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
            return await _context.Quotes.Include(q => q.QuoteTags).ThenInclude(qt => qt.Tags).ToListAsync();
        }

        public async Task<Quote> GetQuoteByIdAsync(int id)
        {
            return await _context.Quotes.Include(q => q.QuoteTags).ThenInclude(qt => qt.Tags).FirstOrDefaultAsync(q => q.Id == id);
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

        //public async Task DeleteQuoteAsync(int id)
        //{
        //    var quote = await _context.Quotes.FindAsync(id);
        //    if (quote != null)
        //    {
        //        _context.Quotes.Remove(quote);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task DeleteQuoteAsync(int id)
        {
            // Fetch the quote with related QuoteTags
            var quote = await _context.Quotes
                .Include(q => q.QuoteTags)
                .ThenInclude(qt => qt.Tags)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote != null)
            {
                // Remove associated entries in QuoteTags
                _context.QuotesTags.RemoveRange(quote.QuoteTags);

                // Remove the quote itself
                _context.Quotes.Remove(quote);

                // Save changes to delete quote and QuoteTags
                await _context.SaveChangesAsync();

                // Optionally: Clean up unused tags after the quote and QuoteTags are deleted
                await CleanupUnusedTagsAsync();
            }
            else
            {
                throw new Exception("Quote not found");
            }
        }
        private async Task CleanupUnusedTagsAsync()
        {
            // Find all tags that are no longer associated with any quotes
            var unusedTags = await _context.Tags
                .Where(tag => !_context.QuotesTags.Any(qt => qt.TagId == tag.TagId))
                .ToListAsync();

            if (unusedTags.Any())
            {
                // Remove the unused tags
                _context.Tags.RemoveRange(unusedTags);
                await _context.SaveChangesAsync();
            }
        }




        public async Task<IEnumerable<Quote>> SearchQuotesAsync(string author, List<string> tags, string quote)
        {
            var query = _context.Quotes.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(q => q.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(quote))
            {
                query = query.Where(q => q.QuoteText.ToLower().Contains(quote.ToLower()));
            }

            if (tags != null && tags.Any())
            {
                var tagsList = tags.Where(tag => !string.IsNullOrEmpty(tag)).Select(tag => tag.ToLower().Trim()).ToList();

                if (tagsList.Any(tag => tag.Contains(",")))
                {
                    tagsList = tagsList.SelectMany(tag => tag.Split(',')).ToList();
                }
                query = query.Where(q => q.QuoteTags.Any(qt => tagsList.Contains(qt.Tags.TagName.ToLower())));
            }

            var result = await query.Include(q => q.QuoteTags).ThenInclude(qt => qt.Tags).ToListAsync();
            return result;
        }

        public async Task<List<string>> GetAllTagNamesAsync()
        {
            return await _context.Tags.Select(t => t.TagName).ToListAsync();
        }

        //public async Task<Tags> AddTagAsync(string tagName)
        //{
        //    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => string.Equals(t.TagName, tagName, StringComparison.OrdinalIgnoreCase));
        //    if (existingTag == null)
        //    {
        //        var newTag = new Tags { TagName = tagName };
        //        _context.Tags.Add(newTag);
        //        await _context.SaveChangesAsync();
        //        return newTag;
        //    }
        //    return existingTag;
        //}
        public async Task<Tags> AddTagAsync(string tagName)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName.ToLower() == tagName.ToLower());
            if (existingTag == null)
            {
                var newTag = new Tags { TagName = tagName };
                _context.Tags.Add(newTag);
                await _context.SaveChangesAsync();
                return newTag;
            }
            return existingTag;
        }


        public async Task<IEnumerable<Tags>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

    }
}
