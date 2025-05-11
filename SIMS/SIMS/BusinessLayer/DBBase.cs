namespace SIMS
{
    public class DBBase
    {
        public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("postgresdb") ?? "Host=localhost;Username=postgresadmin;Password=1234;Database=db1";
    }
}
