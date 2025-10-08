using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BookHistoryDto
    {
        public int IssueId { get; set; }
        public string BookTitle { get; set; }
        public string MemberName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? FineAmount { get; set; }

    }
}
