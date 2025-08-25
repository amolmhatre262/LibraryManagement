using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class OverdueMember
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string BookTitle { get; set; }
        public DateTime DueDate { get; set; }
        public int FineAmount { get; set; }
    }
}
