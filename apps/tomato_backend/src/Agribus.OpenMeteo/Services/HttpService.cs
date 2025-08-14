using System.Web;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.OpenMeteo.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> GetAsync(string url, Dictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("The url can not be empty", nameof(url));

        try
        {
            var urlWithParameters = BuildUrlWithParameters(url, parameters);

            var response = await _httpClient.GetAsync(urlWithParameters);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"HTTP request error to {url}: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new TimeoutException($"Request Timeout to {url}", ex);
        }
    }

    private static string BuildUrlWithParameters(
        string baseUrl,
        Dictionary<string, string>? parameters
    )
    {
        if (parameters == null || parameters.Count == 0)
            return baseUrl;

        var uriBuilder = new UriBuilder(baseUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var parameter in parameters)
        {
            if (!string.IsNullOrEmpty(parameter.Key))
            {
                query[parameter.Key] = parameter.Value;
            }
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
