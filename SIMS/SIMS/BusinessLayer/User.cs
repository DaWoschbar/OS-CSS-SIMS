using Npgsql;

namespace SIMS
{
    public class User : DBBase
    {
        public int User_id { get; set; }
        public bool IsActive { get; set; } = true;
        public string Username { get; set; } = "";
        public string PWDHash { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public DateTime LastLogin { get; set; } = DateTime.Now;

        public User() { }

        public User(int id)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from sims.simsuser where user_id = {id}", db))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            User_id = Convert.ToInt32(reader["User_id"]);
                            IsActive = Convert.ToBoolean(reader["IsActive"]);
                            IsAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                            Username = (string)reader["Username"];
                            PWDHash = (string)reader["PWDHash"];
                            LastLogin = Convert.ToDateTime(reader["LastLogin"]);
                        }
                    }
                }
                db.Close();
            }
        }

        public List<User> GetList()
        {
            List<User> result = new List<User>();
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from sims.simsuser order by Username", db))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User item = new User()
                            {
                                User_id = Convert.ToInt32(reader["User_id"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                                Username = (string)reader["Username"],
                                PWDHash = (string)reader["PWDHash"],
                                LastLogin = Convert.ToDateTime(reader["LastLogin"])
                            };
                            result.Add(item);
                        }
                    }
                }
                db.Close();
            }
            return result;
        }

        public void Save()
        {
            using (NpgsqlConnection db = new NpgsqlConnection(base.ConnectionString))
            {
                db.Open();
                string sql = "";
                if (User_id == 0)
                {
                    sql += $"insert into sims.simsuser(IsAdmin, IsActive, Username, PWDHash, LastLogin) ";
                    sql += $"VALUES (@IsAdmin, @IsActive, @Username, @PWDHash, @LastLogin);";
                }
                else
                {
                    sql += $"update sims.simsuser set IsActive = @IsActive, IsAdmin = @IsAdmin, Username = @Username, ";
                    sql += $"PWDHash = @PWDHash, LastLogin = @LastLogin where User_id = {User_id};";
                }
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, db))
                {
                    cmd.Parameters.AddWithValue("IsActive", IsActive);
                    cmd.Parameters.AddWithValue("IsAdmin", IsAdmin);
                    cmd.Parameters.AddWithValue("Username", Username);
                    cmd.Parameters.AddWithValue("PWDHash", PWDHash);
                    cmd.Parameters.AddWithValue("LastLogin", LastLogin);
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }
    }
}
