using System.Text.Json.Serialization;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Domain.AggregatesModels.SensorAggregates;

public class Sensor : BaseEntity
{
    public required string Name { get; set; }
    public required string SourceAddress { get; set; }
    public required SensorModel Model { get; set; }
    public bool? IsActive { get; set; } = true;

    [JsonIgnore]
    public Guid GreenhouseId { get; set; }

    [JsonIgnore]
    public Greenhouse? Greenhouse { get; set; }

    public void Update(UpdateSensorDto dto)
    {
        Name = dto.Name ?? Name;
        Model = dto.Model ?? Model;
        IsActive = dto.IsActive ?? IsActive;

        UpdateLastModified();
    }
}
