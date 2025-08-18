using Agribus.Application.GreenhouseUsecases;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;
using NSubstitute;

namespace Agribus.UnitTests.GreenhouseUsecase;

public class GreenhouseUsecaseTests
{
    [Fact]
    public async Task ShouldReturnUserGreenhouses()
    {
        // Given
        var userId = "user_123";
        var ct = CancellationToken.None;

        var olderGreenhouse = GreenhouseFactory.Build("Serre Sud", userId);
        var newerGreenhouse = GreenhouseFactory.Build("Serre Nord", userId);

        var repository = Substitute.For<IGreenhouseRepository>();
        repository
            .GetByUserIdAsync(userId, ct)
            .Returns(
                [
                    new GreenhouseListItemDto(olderGreenhouse.Id, olderGreenhouse.Name),
                    new GreenhouseListItemDto(newerGreenhouse.Id, newerGreenhouse.Name),
                ]
            );

        var usecase = new GetUserGreenhousesUsecase(repository);

        // When
        var result = (await usecase.Handle(userId, ct)).ToList();

        // Then
        await repository.Received(1).GetByUserIdAsync(userId, Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Assert tri DESC by CreatedAt
        Assert.Equal("Serre Sud", result[0].Name);
        Assert.Equal("Serre Nord", result[1].Name);
    }

    [Fact]
    public async Task ShouldReturnGreenhouse_WhenExists()
    {
        // Given
        var userId = "user_123";
        var ct = CancellationToken.None;
        var greenhouse = GreenhouseFactory.Build("Serre Sud", userId);

        var repository = Substitute.For<IGreenhouseRepository>();
        repository.GetByIdAsync(greenhouse.Id, userId, ct).Returns(greenhouse);

        var usecase = new GetGreenhouseByIdUsecase(repository);

        // When
        var result = await usecase.Handle(greenhouse.Id, userId, ct);

        // Then
        await repository
            .Received(1)
            .GetByIdAsync(greenhouse.Id, userId, Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        Assert.Equal(result.Id, greenhouse.Id);
        Assert.Equal(result.Name, greenhouse.Name);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenGreenhouseNotFound()
    {
        var userId = "user_123";
        Guid fakeId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var repository = Substitute.For<IGreenhouseRepository>();
        repository
            .GetByIdAsync(fakeId, userId, Arg.Any<CancellationToken>())
            .Returns((Greenhouse?)null);

        var usecase = new GetGreenhouseByIdUsecase(repository);

        // When
        var result = await usecase.Handle(fakeId, userId, ct);

        // Then
        await repository.Received(1).GetByIdAsync(fakeId, userId, Arg.Any<CancellationToken>());
        Assert.Null(result);
    }

    [Fact]
    public async Task ShouldCreateGreenhouse_GivenValidInput()
    {
        // Given
        var fakeUserId = "user_12345";
        var authContext = Substitute.For<IAuthService>();
        authContext.GetCurrentUserId().Returns(fakeUserId);

        var geocodingApiService = Substitute.For<IGeocodingApiService>();

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
                    ImageUrl = "imageUrl",
                },
            ],
            Sensors = [],
        };

        greenhouseRepository
            .AddAsync(Arg.Any<Greenhouse>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Greenhouse>());

        geocodingApiService
            .GetCoordinatesAsync(Arg.Any<String>(), Arg.Any<String>())
            .Returns(("45.74846", "4.84671"));

        var usecase = new CreateGreenhouseUsecase(greenhouseRepository, geocodingApiService);

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
            Crops = [],
            UserId = fakeUserId,
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
            UserId = userId,
        };
        var dto = new UpdateGreenhouseDto
        {
            Name = "New Name",
            City = "Paris",
            Country = "France",
            Crops = [],
        };
        var geocodingApiService = Substitute.For<IGeocodingApiService>();
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
        geocodingApiService
            .GetCoordinatesAsync(Arg.Any<String>(), Arg.Any<String>())
            .Returns(("48.85341", "2.3488"));

        var usecase = new UpdateGreenhouseUsecase(greenhouseRepository, geocodingApiService);

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

static class GreenhouseFactory
{
    public static Greenhouse Build(string name, string userId) =>
        new()
        {
            Name = name,
            City = "City",
            Country = "Country",
            Crops = new(),
            UserId = userId,
        };
}
