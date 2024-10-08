﻿using System.ComponentModel.DataAnnotations;

namespace QuotesManagement.Domain.DTO
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
