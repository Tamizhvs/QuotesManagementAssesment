using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
