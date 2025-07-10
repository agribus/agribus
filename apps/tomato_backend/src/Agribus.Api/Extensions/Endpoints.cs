public class Endpoints
{
    private const string ApiBasePath = "api";

    public static class Ping
    {
        public const string Index = $"{ApiBasePath}/ping";
    }

    public static class Sensors
    {
        private const string BasePath = $"{Endpoints.ApiBasePath}/sensors";
        public const string PushSensorData = $"{BasePath}/data";
    }
}
