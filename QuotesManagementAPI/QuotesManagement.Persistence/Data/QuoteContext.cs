using Microsoft.EntityFrameworkCore;
using QuotesManagement.Domain.Entity;

namespace QuotesManagement.Persistence.Data
{
    public class QuoteContext : DbContext
    {
        public QuoteContext(DbContextOptions<QuoteContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
    }
}
