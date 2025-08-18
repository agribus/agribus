using System.Web;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.OpenMeteo.Services;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory =
            httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<string> GetAsync(string url, Dictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("The url can not be empty", nameof(url));

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var urlWithParameters = BuildUrlWithParameters(url, parameters);

            var response = await httpClient.GetAsync(urlWithParameters);
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
