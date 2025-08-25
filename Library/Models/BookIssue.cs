using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BookIssue
    {
        public int IssueId { get; set; }

        [Required(ErrorMessage = "Please select a book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please select a member")]
        public int MemberId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? FineAmount { get; set; }

        // Optional for displaying in view
        public string BookTitle { get; set; }
        public string MemberName { get; set; }

    }
}
