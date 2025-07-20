using Agribus.Application.GreenhouseUsecases;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using NSubstitute;

namespace Agribus.UnitTests.GreenhouseUsecase;

public class GreenhouseUsecaseTests
{
    [Fact]
    public async Task ShouldCreateGreenhouse_GivenValidInput()
    {
        // Given
        var fakeUserId = Guid.NewGuid();
        var authContext = Substitute.For<IAuthContextService>();
        authContext.GetCurrentUserId().Returns(fakeUserId);

        var greenhouseRepository = Substitute.For<IGreenhouseRepository>();

        var dto = new CreateGreenhouseDto
        {
            Name = "Serre 01",
            City = "Lyon",
            Country = "France",
            Crops = new List<Crop>
            {
                new()
                {
                    CommonName = "Y",
                    ScientificName = "Y",
                    Quantity = 3,
                },
                new()
                {
                    CommonName = "X",
                    ScientificName = "X",
                    Quantity = 2,
                },
            },
            Sensors = new List<CreateSensorDto>(),
        };

        var expectedGreenhouse = dto.MapToGreenhouse();
        greenhouseRepository
            .AddAsync(Arg.Any<Greenhouse>(), fakeUserId, Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Greenhouse>());

        // var usecase = new CreateGreenhouseUsecase(authContext, greenhouseRepository);
        var usecase = new CreateGreenhouseUsecase(greenhouseRepository);

        // When
        var result = await usecase.Handle(dto, fakeUserId, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.City, result.City);
        Assert.Equal(dto.Country, result.Country);
        Assert.Equal(dto.Crops.Count, result.Crops.Count);
        Assert.Equal(dto.Sensors.Count, result.Sensors.Count);
    }

    [Fact]
    public async Task ShouldDeleteGreenhouse_GivenValidInput()
    {
        // Given
        var fakeUserId = Guid.NewGuid();
        var greenhouseRepository = Substitute.For<IGreenhouseRepository>();
        var greenhouse = new Greenhouse
        {
            Name = "Test Greenhouse",
            City = "Paris",
            Country = "France",
            Crops = new List<Crop>(),
        };
        greenhouseRepository
            .Exists(greenhouse.Id, fakeUserId, CancellationToken.None)
            .Returns(greenhouse);
        greenhouseRepository.DeleteAsync(greenhouse, CancellationToken.None).Returns(true);

        var usecase = new DeleteGreenhouseUsecase(greenhouseRepository);

        // When
        await usecase.Handle(greenhouse.Id, fakeUserId, CancellationToken.None);

        // Then
        await greenhouseRepository.Received(1).DeleteAsync(greenhouse, CancellationToken.None);
    }

    [Fact]
    public async Task ShouldUpdateGreenhouse_GivenValidInput()
    {
        // Given
        var greenhouseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var originalGreenhouse = new Greenhouse
        {
            Name = "Old Name",
            City = "Old City",
            Country = "Old Country",
            Crops = new List<Crop>(),
        };

        var dto = new UpdateGreenhouseDto
        {
            Name = "New Name",
            City = "New City",
            Country = "New Country",
            Crops = new List<Crop> { },
            Sensors = new List<UpdateSensorDto>(),
        };

        var greenhouseRepository = Substitute.For<IGreenhouseRepository>();
        greenhouseRepository
            .Exists(originalGreenhouse.Id, userId, CancellationToken.None)
            .Returns(originalGreenhouse);

        greenhouseRepository
            .UpdateAsync(Arg.Any<Greenhouse>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var usecase = new UpdateGreenhouseUsecase(greenhouseRepository);

        // When
        var result = await usecase.Handle(greenhouseId, userId, dto, CancellationToken.None);

        // Then
        await greenhouseRepository
            .Received(1)
            .UpdateAsync(
                Arg.Is<Greenhouse>(g =>
                    g.Id == greenhouseId
                    && g.Name == dto.Name
                    && g.City == dto.City
                    && g.Country == dto.Country
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
