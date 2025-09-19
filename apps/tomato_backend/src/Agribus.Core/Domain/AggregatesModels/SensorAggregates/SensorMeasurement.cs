using System.Text.Json.Serialization;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Domain.AggregatesModels.SensorAggregates;

public class SensorMeasurement
{
    public required long Date { get; set; }
    public required double Value { get; set; }
    public required string SourceAddress { get; set; }
    public required SensorType Type { get; set; }
}

public class GetSensorMeasurementDto
{
    public required DateTime Date { get; set; }
    public required double Value { get; set; }
    public required string SourceAddress { get; set; }
    public required SensorType Type { get; set; }
}

public class MeasureValueDto
{
    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "";

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
}

public class MetricsDto
{
    [JsonPropertyName("temperature")]
    public MeasureValueDto Temperature { get; set; } = new();

    [JsonPropertyName("humidity")]
    public MeasureValueDto Humidity { get; set; } = new();

    [JsonPropertyName("airPressure")]
    public MeasureValueDto AirPressure { get; set; } = new();
}

public class SummaryAggregatesDto
{
    [JsonPropertyName("metrics")]
    public MetricsDto Metrics { get; set; } = new();
}

public class SensorLastDto
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("temperature")]
    public MeasureValueDto? Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public MeasureValueDto? Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public MeasureValueDto? Pressure { get; set; }
}

public class SensorCardDto
{
    [JsonPropertyName("sourceAddress")]
    public string SourceAddress { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("last")]
    public SensorLastDto Last { get; set; } = new();
}

public class LatestMeasurementsResponseDto
{
    [JsonPropertyName("summaryAggregates")]
    public SummaryAggregatesDto SummaryAggreglates { get; set; } = new();

    [JsonPropertyName("sensors")]
    public List<SensorCardDto> Sensors { get; set; } = new();
}
