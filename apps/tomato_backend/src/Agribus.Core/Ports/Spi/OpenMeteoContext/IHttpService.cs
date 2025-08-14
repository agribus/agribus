namespace Agribus.Core.Ports.Spi.OpenMeteoContext
{
    public interface IHttpService
    {
        Task<string> GetAsync(string url, Dictionary<string, string>? parameters);
    }
}
