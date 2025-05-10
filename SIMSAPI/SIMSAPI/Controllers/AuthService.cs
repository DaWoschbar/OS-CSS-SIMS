using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Npgsql;

namespace SIMSAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthService : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        [Route("check")]
        public IActionResult Get(string username, string token)
        {
            if (new RedisDB().CheckToken(username, token) == true)
            {
                return Ok();
            }

            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public string Post(string username, string password)
        {
            if (checkUser(username, password))
            {
                string token = generateToken(username, password);
                new RedisDB().StoreToken(username, token);
                return token;
            }
            else
            {
                return "";
            }
        }

        private string generateToken(string username, string password)
        {
            //HACK generate cool Token ;-) -> base64 is not encryption!
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + password + DateTime.Now.ToString()));
        }

        private bool checkUser(string username, string password)
        {
            try
            {
                bool result = false;
                string connectionString = Environment.GetEnvironmentVariable("postgresdb") ?? "Host=localhost;Username=postgresadmin;Password=1234;Database=db1";

                using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
                {
                    db.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from sims.simsuser where username = @username and pwdhash = @pwdhash", db))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("pwdhash", computeSha256Hash(password));
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = reader.HasRows;
                        }
                    }
                    db.Close();
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        private string computeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                return builder.ToString();
            }

        }
    }
}