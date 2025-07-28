public class Endpoints
{
    private const string ApiBasePath = "api";

    public static class Ping
    {
        public const string Index = $"{ApiBasePath}/ping";
        public const string Private = $"{ApiBasePath}/private/ping";
    }

    public static class Sensors
    {
        private const string BasePath = $"{Endpoints.ApiBasePath}/sensors";
        public const string PushSensorData = $"{BasePath}/data";
    }

    public static class User
    {
        private const string BasePath = $"{ApiBasePath}/users";
        public const string Login = $"{BasePath}/login";
        public const string Signup = $"{BasePath}/signup";
        public const string Logout = $"{BasePath}/logout";
    }
}
