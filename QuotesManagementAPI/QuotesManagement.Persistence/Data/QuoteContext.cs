using Microsoft.EntityFrameworkCore;
using QuotesManagement.Domain.Entity;

namespace QuotesManagement.Persistence.Data
{
    public class QuoteContext : DbContext
    {
        public QuoteContext(DbContextOptions<QuoteContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<QuoteTags> QuotesTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuoteTags>()
                .HasKey(qt => new { qt.QuoteId, qt.TagId });

            modelBuilder.Entity<QuoteTags>()
                .HasOne(qt => qt.Quote)
                .WithMany(q => q.QuoteTags)
                .HasForeignKey(qt => qt.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuoteTags>()
                .HasOne(qt => qt.Tags)
                .WithMany(t => t.QuoteTags)
                .HasForeignKey(qt => qt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
