using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBookService.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string TimeStamp { get; set; }
    }
}