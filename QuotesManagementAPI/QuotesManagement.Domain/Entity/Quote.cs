using System.ComponentModel.DataAnnotations;

namespace QuotesManagement.Domain.Entity
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string QuoteText { get; set; }

        public string Tags { get; set; }
    }

}
