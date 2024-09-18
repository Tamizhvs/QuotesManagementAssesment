using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
