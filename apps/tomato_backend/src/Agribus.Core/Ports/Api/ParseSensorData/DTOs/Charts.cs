namespace Agribus.Core.Ports.Api.ParseSensorData.DTOs;

public class ChartTimeseriesResponseDto
{
    public ChartRangeDto Range { get; set; } = new();
    public ChartMetricsDto Metrics { get; set; } = new();
}

public class ChartRangeDto
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
}

public class ChartMetricsDto
{
    public MetricSeriesDto Temperature { get; set; } =
        new()
        {
            Unit = "°C",
            YMin = 0,
            YMax = 50,
        };
    public MetricSeriesDto Humidity { get; set; } =
        new()
        {
            Unit = "%",
            YMin = 0,
            YMax = 100,
        };
    public MetricSeriesDto AirPressure { get; set; } =
        new()
        {
            Unit = "hPa",
            YMin = 950,
            YMax = 1050,
        };
}

public class MetricSeriesDto
{
    public string Unit { get; set; } = "";
    public double YMin { get; set; }
    public double YMax { get; set; }
    public List<MetricPointDto> Points { get; set; } = new();
}

public class MetricPointDto
{
    public string Date { get; set; } = "";
    public double? Value { get; set; }
}
