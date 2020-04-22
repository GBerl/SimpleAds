using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SimpleAds.Data
{
    public class Adsdb
    {
        private string _connection;
        public Adsdb(string connection)
        {
            _connection = connection;
        }
        public List<Ad> GetAds()
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = @"Select * from Ads";

                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Ad> ads = new List<Ad>();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        Number = (string)reader["Number"],
                        UserId = (int)reader["UserId"]
                    });

                }
                return ads;
            }
        }
        public void AddUser(string name, string email, string password)
        {
            string hashedpassword = BCrypt.Net.BCrypt.HashPassword(password);
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = @"insert into Users (Name, Email, PasswordHashed) 
                                    values (@name, @email, @hashedpassword)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hashedpassword", hashedpassword);
                sql.Open();
                cmd.ExecuteNonQuery();

            }
        }
        public User Login(string password, string email)
        {
            User user = new User();
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = "select * from users where email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    user.Id = (int)reader["id"];
                    user.Email = (string)reader["email"];
                    user.Name = (string)reader["name"];
                    user.Hash = (string)reader["passwordhashed"];


                };
                if (BCrypt.Net.BCrypt.Verify(password, user.Hash))
                {
                    return user;
                }

                return null;
            }
        }
        public void NewAd(Ad a)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                int id = GetIdByEmail(a.UserEmail);
                cmd.CommandText = @"insert into ads (Title, Description, Number, UserId)
                                    values(@title, @description, @number, @id)";
                sql.Open();
                cmd.Parameters.AddWithValue("@title", a.Title);
                cmd.Parameters.AddWithValue("@description", a.Description);
                cmd.Parameters.AddWithValue("@number", a.Number);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

            }
        }
        public int GetIdByEmail(string email)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = "select id from users where email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                sql.Open();
                int id = (int)cmd.ExecuteScalar();
                return id;
            }
        }
        public void Delete(int id, int userid)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = "Delete from Ads where id=@id and UserId=@userid";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@userid", userid);

                sql.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<Ad> GetAds(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            using (SqlCommand cmd = sql.CreateCommand())
            {
                cmd.CommandText = @"Select * from Ads
                                    where userid=@id";
                cmd.Parameters.AddWithValue("@id", id);
                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Ad> ads = new List<Ad>();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        Number = (string)reader["Number"],
                        UserId = (int)reader["UserId"]
                    });

                }
                return ads;
            }
        }

    }
}
