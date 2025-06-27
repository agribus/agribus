public class Endpoints
{
    private const string ApiBasePath = "api";

    public static class Sensors
    {
        private const string BasePath = $"{Endpoints.ApiBasePath}/sensors";
        public const string PushSensorData = $"{BasePath}/data";
    }
}
