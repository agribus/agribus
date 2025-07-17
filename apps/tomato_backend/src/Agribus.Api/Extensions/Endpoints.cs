namespace Agribus.Api.Extensions;

public static class Endpoints
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

    public static class Greenhouses
    {
        private const string BasePath = $"{ApiBasePath}/greenhouses";
        public const string GetUserGreenhouses = $"{BasePath}";
        public const string Create = $"{BasePath}/create";
        public const string Delete = $"{BasePath}/{{id:guid}}";
        public const string Update = $"{BasePath}/update/{{id:guid}}";
    }
}
