using Microsoft.Data.SqlClient;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repositories
{
    public class ReportsRepository
    {
        private readonly string _connectionString;

        public ReportsRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<OverdueMember> GetOverdueMembers()
        {
            var overdueList = new List<OverdueMember>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT m.MemberId, m.FullName, m.Email, b.Title AS BookTitle,
                   bi.DueDate, DATEDIFF(DAY, bi.DueDate, GETDATE()) * 10 AS FineAmount
            FROM BookIssue bi
            INNER JOIN Members_Master m ON bi.MemberId = m.MemberId
            INNER JOIN Books_Master b ON bi.BookId = b.BookId
            WHERE bi.ReturnDate IS NULL AND bi.DueDate < GETDATE()";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    overdueList.Add(new OverdueMember
                    {
                        MemberId = Convert.ToInt32(reader["MemberId"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        BookTitle = reader["BookTitle"].ToString(),
                        DueDate = Convert.ToDateTime(reader["DueDate"]),
                        FineAmount = Convert.ToInt32(reader["FineAmount"])
                    });
                }
            }
            return overdueList;
        }


        public List<BookHistory> GetBookHistory(int bookId)
        {
            var history = new List<BookHistory>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT bi.IssueId, m.FullName AS MemberName, bi.IssueDate, bi.DueDate, bi.ReturnDate, bi.FineAmount
            FROM BookIssue bi
            INNER JOIN Members_Master m ON bi.MemberId = m.MemberId
            WHERE bi.BookId = @BookId
            ORDER BY bi.IssueDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    history.Add(new BookHistory
                    {
                        IssueId = Convert.ToInt32(reader["IssueId"]),
                        MemberName = reader["MemberName"].ToString(),
                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                        DueDate = Convert.ToDateTime(reader["DueDate"]),
                        ReturnDate = reader["ReturnDate"] == DBNull.Value ? null : (DateTime?)reader["ReturnDate"],
                        FineAmount = reader["FineAmount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FineAmount"])
                    });
                }
            }
            return history;
        }



    }
}
