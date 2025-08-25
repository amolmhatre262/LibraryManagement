using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Library.Repositories
{
    public class BooksRepository
    {
        private readonly string _connectionString;

        public BooksRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Books> GetAllBooks()
        {
            var books = new List<Books>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Books_Master", con);
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

        // 🔹 Get book by ID
        public Books GetBookById(int id)
        {
            Books book = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Books_Master WHERE BookId=@BookId", con);
                cmd.Parameters.AddWithValue("@BookId", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    book = new Books
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        Title = reader["Title"].ToString(),
                        Author = reader["Author"].ToString(),
                        ISBN = reader["ISBN"].ToString(),
                        TotalCopies = Convert.ToInt32(reader["TotalCopies"]),
                        AvailableCopies = Convert.ToInt32(reader["AvailableCopies"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                }
            }
            return book;
        }

        // 🔹 Insert new book
        public void AddBook(Books book)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Books_Master (Title, Author, ISBN, TotalCopies, AvailableCopies, CreatedDate) 
                                 VALUES (@Title, @Author, @ISBN, @TotalCopies, @AvailableCopies, @CreatedDate)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                cmd.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
                cmd.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 Update existing book
        public void UpdateBook(Books book)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Books_Master 
                                 SET Title=@Title, Author=@Author, ISBN=@ISBN, TotalCopies=@TotalCopies, AvailableCopies=@AvailableCopies
                                 WHERE BookId=@BookId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                cmd.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
                cmd.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);
                cmd.Parameters.AddWithValue("@BookId", book.BookId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 Delete book by ID
        public void DeleteBook(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Books_Master WHERE BookId=@BookId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BookId", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
