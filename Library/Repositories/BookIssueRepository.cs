using Library.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repositories
{
    public class BookIssueRepository
    {
        private readonly string _connectionString;

        public BookIssueRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        
        public void IssueBook(BookIssue issue)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand checkCmd = new SqlCommand("SELECT AvailableCopies FROM Books_Master WHERE BookId=@BookId", con);
                checkCmd.Parameters.AddWithValue("@BookId", issue.BookId);
                con.Open();
                int available = (int)checkCmd.ExecuteScalar();

                if (available <= 0)
                    throw new Exception("No copies available");

                // Insert issue record
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO BookIssue (BookId, MemberId, IssueDate, DueDate) VALUES (@BookId,@MemberId,@IssueDate,@DueDate)", con);
                cmd.Parameters.AddWithValue("@BookId", issue.BookId);
                cmd.Parameters.AddWithValue("@MemberId", issue.MemberId);
                cmd.Parameters.AddWithValue("@IssueDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(7));
                cmd.ExecuteNonQuery();

                // Update AvailableCopies
                SqlCommand updateCmd = new SqlCommand("UPDATE Books_Master SET AvailableCopies = AvailableCopies - 1 WHERE BookId=@BookId", con);
                updateCmd.Parameters.AddWithValue("@BookId", issue.BookId);
                updateCmd.ExecuteNonQuery();
            }
        }

        public List<Books> GetAvailableBooks()
        {
            var books = new List<Books>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Books_Master WHERE AvailableCopies > 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Books
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        Title = reader["Title"].ToString(),
                        Author = reader["Author"].ToString(),
                        ISBN = reader["ISBN"].ToString(),
                        TotalCopies = Convert.ToInt32(reader["TotalCopies"]),
                        AvailableCopies = Convert.ToInt32(reader["AvailableCopies"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }

            return books;
        }


        public List<Members> GetAllMembers()
        {
            var members = new List<Members>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Members_Master WHERE IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    members.Add(new Members
                    {
                        MemberId = Convert.ToInt32(reader["MemberId"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Mobile = reader["Mobile"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }

            return members;
        }

        public void ReturnBook(int issueId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // 1. Get the issued book record
                SqlCommand cmd = new SqlCommand(
                    "SELECT BookId, DueDate FROM BookIssue WHERE IssueId=@IssueId AND ReturnDate IS NULL", con);
                cmd.Parameters.AddWithValue("@IssueId", issueId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    throw new Exception("This book is either already returned or issue record not found.");

                int bookId = Convert.ToInt32(reader["BookId"]);
                DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);
                reader.Close();

                // 2. Calculate fine
                DateTime returnDate = DateTime.Now;
                decimal fine = 0;
                if (returnDate > dueDate)
                {
                    int daysLate = (returnDate - dueDate).Days;
                    fine = daysLate * 10; // ₹10 per day
                }

                // 3. Update BookIssue record
                SqlCommand updateCmd = new SqlCommand(
                    "UPDATE BookIssue SET ReturnDate=@ReturnDate, FineAmount=@FineAmount WHERE IssueId=@IssueId", con);
                updateCmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                updateCmd.Parameters.AddWithValue("@FineAmount", fine);
                updateCmd.Parameters.AddWithValue("@IssueId", issueId);
                updateCmd.ExecuteNonQuery();

                // 4. Increment AvailableCopies in Books
                SqlCommand updateBookCmd = new SqlCommand(
                    "UPDATE Books_Master SET AvailableCopies = AvailableCopies + 1 WHERE BookId=@BookId", con);
                updateBookCmd.Parameters.AddWithValue("@BookId", bookId);
                updateBookCmd.ExecuteNonQuery();
            }
        }

        public List<BookIssue> GetActiveIssues()
        {
            var issues = new List<BookIssue>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT bi.IssueId, bi.BookId, bi.MemberId, bi.IssueDate, bi.DueDate,
                   b.Title AS BookTitle, m.FullName AS MemberName
            FROM BookIssue bi
            INNER JOIN Books_Master b ON bi.BookId = b.BookId
            INNER JOIN Members_Master m ON bi.MemberId = m.MemberId
            WHERE bi.ReturnDate IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    issues.Add(new BookIssue
                    {
                        IssueId = Convert.ToInt32(reader["IssueId"]),
                        BookId = Convert.ToInt32(reader["BookId"]),
                        MemberId = Convert.ToInt32(reader["MemberId"]),
                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                        DueDate = Convert.ToDateTime(reader["DueDate"]),
                        BookTitle = reader["BookTitle"].ToString(),
                        MemberName = reader["MemberName"].ToString()
                    });
                }
            }

            return issues;
        }

        public List<IssueStatsDto> GetBookIssuesLast7Days()
        {
            var list = new List<IssueStatsDto>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
            SELECT CONVERT(varchar, IssueDate, 23) AS Date, COUNT(*) AS Count
            FROM BookIssue
            WHERE IssueDate >= DATEADD(DAY, -6, CAST(GETDATE() AS date))
            GROUP BY CONVERT(varchar, IssueDate, 23)
            ORDER BY Date;";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new IssueStatsDto
                        {
                            Date = reader["Date"].ToString(),
                            Count = Convert.ToInt32(reader["Count"])
                        });
                    }
                }
            }

            return list;
        }

        public List<CategoryStatsDto> GetBooksByCategory()
        {
            var list = new List<CategoryStatsDto>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
            SELECT b.Category, COUNT(*) AS Count
            FROM BookIssue bi
            INNER JOIN Books_Master b ON bi.BookId = b.BookId
            GROUP BY b.Category;
        ";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CategoryStatsDto
                        {
                            Category = reader["Category"].ToString(),
                            Count = Convert.ToInt32(reader["Count"])
                        });
                    }
                }
            }

            return list;
        }

        // bellow this for dashboard

        public int GetTotalBooks()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Books_Master", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetTotalMembers()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Members_Master", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetTotalIssuedBooks()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BookIssue WHERE ReturnDate IS NULL", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetOverdueBooksCount()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BookIssue WHERE ReturnDate IS NULL AND DueDate < GETDATE()", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        //public IEnumerable<BookIssue> GetAllIssuedBooks()
        //{
        //    var issuedBooks = new List<BookIssue>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();
        //        string sql = @"
        //    SELECT bi.IssueId, bi.BookId, bi.MemberId, bi.IssueDate, bi.ReturnDate, bi.FineAmount,
        //           b.Title, m.FullName
        //    FROM BookIssue bi
        //    INNER JOIN Books_Master b ON bi.BookId = b.BookId
        //    INNER JOIN Members_Master m ON bi.MemberId = m.MemberId
        //    ORDER BY bi.IssueDate DESC";

        //        using (SqlCommand cmd = new SqlCommand(sql, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                issuedBooks.Add(new BookIssue
        //                {
        //                    IssueId = (int)reader["IssueId"],
        //                    BookId = (int)reader["BookId"],
        //                    MemberId = (int)reader["MemberId"],
        //                    IssueDate = (DateTime)reader["IssueDate"],
        //                    ReturnDate = reader["ReturnDate"] as DateTime?,
        //                    FineAmount = reader["FineAmount"] != DBNull.Value ? Convert.ToDecimal(reader["FineAmount"]) : 0,
        //                    BookTitle = reader["Title"].ToString(),
        //                    MemberName = reader["FullName"].ToString()
        //                });
        //            }
        //        }
        //    }

        //    return issuedBooks;
        //}

        public List<OverdueMember> GetOverdueMembersReport()
        {
            var overdueList = new List<OverdueMember>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT m.MemberId, m.FullName, b.Title AS BookTitle,
                   bi.DueDate, bi.FineAmount
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
                        BookTitle = reader["BookTitle"].ToString(),
                        DueDate = Convert.ToDateTime(reader["DueDate"]),
                        FineAmount = reader["FineAmount"] == DBNull.Value? 0: Convert.ToInt32(reader["FineAmount"])
                    });
                }
            }

            return overdueList;
        }

        public List<BookHistoryDto> GetBookHistory(int bookId)
        {
            var history = new List<BookHistoryDto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            SELECT bi.IssueId, b.Title AS BookTitle, m.FullName AS MemberName,
                   bi.IssueDate, bi.DueDate, bi.ReturnDate, bi.FineAmount
            FROM BookIssues bi
            INNER JOIN Books_Master b ON bi.BookId = b.BookId
            INNER JOIN Members m ON bi.MemberId = m.MemberId
            WHERE bi.BookId = @BookId
            ORDER BY bi.IssueDate DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookId", bookId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new BookHistoryDto
                            {
                                IssueId = Convert.ToInt32(reader["IssueId"]),
                                BookTitle = reader["BookTitle"].ToString(),
                                MemberName = reader["MemberName"].ToString(),
                                IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                DueDate = Convert.ToDateTime(reader["DueDate"]),
                                ReturnDate = reader["ReturnDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ReturnDate"]),
                                FineAmount = reader["FineAmount"] == DBNull.Value ? null : Convert.ToDecimal(reader["FineAmount"])
                            });
                        }
                    }
                }
            }

            return history;
        }






    }
}
