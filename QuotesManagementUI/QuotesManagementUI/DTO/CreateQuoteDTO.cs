using System.ComponentModel.DataAnnotations;

namespace QuotesManagementUI.DTO
{
    public class CreateQuoteDTO
    {
        [Required]
        public String Author { get; set; }
        [Required]
        public List<string> Tags { get; set; }
        [Required]
        public string Quote { get; set; }
    }
}
