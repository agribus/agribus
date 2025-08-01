using Agribus.Application.GreenhouseUsecases;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
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
        var fakeUserId = "user_12345";
        var authContext = Substitute.For<IAuthService>();
        authContext.GetCurrentUserId().Returns(fakeUserId);

        var greenhouseRepository = Substitute.For<IGreenhouseRepository>();

        var dto = new CreateGreenhouseDto
        {
            Name = "Serre 01",
            City = "Lyon",
            Country = "France",
            Crops =
            [
                new Crop
                {
                    CommonName = "Y",
                    ScientificName = "Y",
                    Quantity = 3,
                    PlantingDate = DateTime.Now,
                },
                new Crop
                {
                    CommonName = "X",
                    ScientificName = "X",
                    Quantity = 2,
                    PlantingDate = DateTime.Now,
                },
            ],
            Sensors = [],
        };

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
        var fakeUserId = "user_67890";
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
        var userId = "user_11111";
        var originalGreenhouse = new Greenhouse
        {
            Name = "Old Name",
            City = "Old City",
            Country = "Old Country",
            Crops = [],
        };
        var dto = new UpdateGreenhouseDto
        {
            Name = "New Name",
            City = "New City",
            Country = "New Country",
            Crops = [],
        };

        var greenhouseRepository = Substitute.For<IGreenhouseRepository>();
        greenhouseRepository
            .Exists(originalGreenhouse.Id, userId, CancellationToken.None)
            .Returns(originalGreenhouse);

        greenhouseRepository
            .UpdateAsync(
                Arg.Any<Greenhouse>(),
                Arg.Any<UpdateGreenhouseDto>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(true);

        var usecase = new UpdateGreenhouseUsecase(greenhouseRepository);

        // When
        var result = await usecase.Handle(
            originalGreenhouse.Id,
            userId,
            dto,
            CancellationToken.None
        );

        // Then
        await greenhouseRepository
            .Received(1)
            .UpdateAsync(originalGreenhouse, dto, CancellationToken.None);
        Assert.True(result);
    }
}
