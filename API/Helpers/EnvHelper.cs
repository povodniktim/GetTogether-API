namespace API.Helpers
{

    public static class EnvVariable
    {
        public static string DbServer => "DB_SERVER";
        public static string DbPort => "DB_PORT";
        public static string DbName => "DB_NAME";
        public static string DbUsername => "DB_USERNAME";
        public static string DbPassword => "DB_PASSWORD";
        public static string DbVersion => "DB_VERSION";
        public static string ApiUsername => "API_USERNAME";
        public static string ApiPassword => "API_PASSWORD";
        public static string JwtSecret => "JWT_SECRET";
        public static string JwtExpirationMinutes => "JWT_EXPIRATION_MINUTES";
    }

    public static class EnvHelper
    {
        public static string GetEnv(string key)
        {
            return Environment.GetEnvironmentVariable(key) ?? throw new Exception($"Environment variable {key} not found");
        }

        public static string GetDatabaseConnectionString()
        {
            return "Server = " + GetEnv(EnvVariable.DbServer) + "; Port = " + GetEnv(EnvVariable.DbPort) + "; Database = " + GetEnv(EnvVariable.DbName) + "; Uid = " + GetEnv(EnvVariable.DbUsername) + "; Pwd = " + GetEnv(EnvVariable.DbPassword) + ";";
        }
    }

}