namespace QuotesManagement.Domain.ResultSet
{
    public class QuoteResult
    {
        public int Id { get; set; }
        public String Author { get; set; }
        public List<string> Tags { get; set; }
        public string Quote { get; set; }
    }
}
