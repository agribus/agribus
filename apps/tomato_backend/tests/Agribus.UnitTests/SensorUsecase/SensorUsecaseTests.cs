using Agribus.Application.SensorUsecases;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.SensorContext;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Agribus.UnitTests.SensorUsecase;

public class SensorUsecaseTests
{
    [Fact]
    public async Task ShouldUpdateSensor_GivenValidInput()
    {
        // Given
        var fakeUserId = Guid.NewGuid();
        var sensorRepository = Substitute.For<ISensorRepository>();

        var originalSensor = new Sensor
        {
            Name = "Capteur Porte",
            Model = SensorModel.RuuviTag,
            SourceAddress = "34499765",
            IsActive = true,
        };
        var dto = new UpdateSensorDto
        {
            Name = "Capteur Est",
            Model = SensorModel.RuuviTagPro,
            IsActive = true,
        };

        sensorRepository
            .Exists(originalSensor.Id, fakeUserId, CancellationToken.None)
            .Returns(originalSensor);
        sensorRepository.UpdateAsync(originalSensor, dto, CancellationToken.None).Returns(true);

        var usecase = new UpdateSensorUsecase(sensorRepository);

        // When
        var result = await usecase.Handle(
            originalSensor.Id,
            fakeUserId,
            dto,
            CancellationToken.None
        );

        // Then
        await sensorRepository.Received(1).UpdateAsync(originalSensor, dto, CancellationToken.None);
    }

    [Fact]
    public async Task ShouldDeleteSensor_GivenValidInput()
    {
        var fakeUserId = Guid.NewGuid();
        var sensorRepository = Substitute.For<ISensorRepository>();
        var usecase = new DeleteSensorUsecase(sensorRepository);
        var originalSensor = new Sensor
        {
            Name = "Capteur Porte",
            Model = SensorModel.RuuviTag,
            SourceAddress = "34499765",
            IsActive = true,
        };

        sensorRepository
            .Exists(originalSensor.Id, fakeUserId, CancellationToken.None)
            .Returns(originalSensor);
        sensorRepository.DeleteAsync(originalSensor, CancellationToken.None).Returns(true);

        // When
        var result = await usecase.Handle(originalSensor.Id, fakeUserId, CancellationToken.None);

        // Then
        await sensorRepository.Received(1).DeleteAsync(originalSensor, CancellationToken.None);
    }
}
