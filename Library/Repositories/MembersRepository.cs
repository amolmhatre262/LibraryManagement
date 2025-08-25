using Library.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repositories
{
    public class MembersRepository
    {
        private readonly string _connectionString;
        public MembersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Members> GetAllMembers()
        {
            var members = new List<Members>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Members_Master", con);
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

        public Members GetMemberByid(int MemberId)
        {
            Members member = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Members_Master WHERE MemberId=@MemberId", con);
                cmd.Parameters.AddWithValue("@MemberId", MemberId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    member = new Members
                    {
                        MemberId = Convert.ToInt32(reader["MemberId"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Mobile = reader["Mobile"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                }
            }
            return member;
        }

        public void AddMember(Members member)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Members_Master (FullName, Email, Mobile, IsActive, CreatedDate) VALUES (@FullName, @Email, @Mobile, @IsActive, @CreatedDate)", con);
                cmd.Parameters.AddWithValue("@FullName", member.FullName);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@Mobile", member.Mobile);
                cmd.Parameters.AddWithValue("@IsActive", member.IsActive);
                cmd.Parameters.AddWithValue("@CreatedDate", member.CreatedDate);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMember(Members member)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Members_Master SET FullName=@FullName, Email=@Email, Mobile=@Mobile, IsActive=@IsActive WHERE MemberId=@MemberId", con);
                cmd.Parameters.AddWithValue("@FullName", member.FullName);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@Mobile", member.Mobile);
                cmd.Parameters.AddWithValue("@IsActive", member.IsActive);
                cmd.Parameters.AddWithValue("@MemberId", member.MemberId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMember(int MemberId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Members_Master WHERE MemberId=@MemberId", con);
                cmd.Parameters.AddWithValue("@MemberId", MemberId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
