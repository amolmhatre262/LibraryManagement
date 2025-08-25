using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class IssueStatsDto
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class CategoryStatsDto
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}
