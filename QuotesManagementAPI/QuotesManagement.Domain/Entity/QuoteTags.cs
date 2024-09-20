namespace QuotesManagement.Domain.Entity
{
    public class QuoteTags
    {
        public int TagId { get; set; }
        public Tags Tags { get; set; }
        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }
}
