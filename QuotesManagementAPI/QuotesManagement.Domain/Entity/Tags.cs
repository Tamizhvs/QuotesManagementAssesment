using System.ComponentModel.DataAnnotations;

namespace QuotesManagement.Domain.Entity
{
    public class Tags
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string TagName { get; set; }
        public ICollection<QuoteTags> QuoteTags { get; set; } = new List<QuoteTags>();
    }
}
