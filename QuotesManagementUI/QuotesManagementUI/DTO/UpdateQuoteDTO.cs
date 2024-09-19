using System.ComponentModel.DataAnnotations;

namespace QuotesManagementUI.DTO
{
    public class UpdateQuoteDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public String Author { get; set; }
        [Required]
        public List<string> Tags { get; set; }
        [Required]
        public string Quote { get; set; }
    }
}
