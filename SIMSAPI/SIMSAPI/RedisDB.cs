using StackExchange.Redis;

namespace SIMSAPI
{
    public class RedisDB
    {
        private string dbname = "";

        public RedisDB() 
        {
            dbname = Environment.GetEnvironmentVariable("redisdb") ?? "localhost";
        }

        public void StoreToken(string username, string token) 
        {
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(dbname);
                IDatabase db = redis.GetDatabase();
                db.StringSet(username, token);
            }
            catch
            {
                throw;
            }
        }

        public bool CheckToken(string username, string token)
        {
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(dbname);
                IDatabase db = redis.GetDatabase();
                return db.StringGet(username).ToString() == token ? true : false;
            }
            catch
            {
                throw;
            }
        }

    }
}
