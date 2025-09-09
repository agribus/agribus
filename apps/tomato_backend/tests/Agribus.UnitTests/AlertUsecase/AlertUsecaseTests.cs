using Agribus.Application.AlertUsecases;
using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Domain.Exceptions;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;
using Agribus.Core.Ports.Api.AlertUsecases.Validators;
using Agribus.Core.Ports.Spi.AlertContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using FluentValidation;
using NSubstitute;

namespace Agribus.UnitTests.AlertUsecase;

public class AlertUsecaseTests
{
    [Fact]
    public async Task ShouldCreateAlert_GivenValidAboveThreshold()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var greenhouseId = Guid.NewGuid();
        var dto = new CreateAlertDto
        {
            GreenhouseId = greenhouseId,
            Name = "Temp > 30°C",
            MeasureType = SensorType.Temperature,
            RuleType = AlertRuleType.Above,
            ThresholdValue = 30.0,
            Enabled = true,
        };

        var repo = Substitute.For<IAlertRepository>();
        repo.AddAsync(Arg.Any<Alert>(), ct).Returns(callInfo => callInfo.Arg<Alert>());

        var usecase = new CreateAlertUsecase(repo, new AlertValidator());

        // When
        var result = await usecase.Handle(dto, userId, ct);

        // Then
        await repo.Received(1).AddAsync(Arg.Any<Alert>(), ct);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.MeasureType, result.MeasureType);
        Assert.Equal(dto.RuleType, result.RuleType);
        Assert.Equal(dto.ThresholdValue, result.ThresholdValue);
        Assert.Equal(dto.GreenhouseId, result.GreenhouseId);
        Assert.Equal(dto.Enabled, result.Enabled);
    }

    [Fact]
    public async Task ShouldRejectCreate_WhenRuleTypeAboveThreshold_AndThresholdMissing()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var dto = new CreateAlertDto
        {
            GreenhouseId = Guid.NewGuid(),
            Name = "Temp > X",
            MeasureType = SensorType.Temperature,
            RuleType = AlertRuleType.Above,
        };

        var repo = Substitute.For<IAlertRepository>();
        var usecase = new CreateAlertUsecase(repo, new AlertValidator());

        // When
        var result = async () => await usecase.Handle(dto, userId, ct);

        // Then
        await Assert.ThrowsAsync<ValidationException>(result);
        await repo.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<Alert>(), CancellationToken.None);
    }

    [Fact]
    public async Task ShouldRejectCreate_WhenRuleTypeOutsideRange_AndMinMaxInvalid()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var dto = new CreateAlertDto
        {
            GreenhouseId = Guid.NewGuid(),
            Name = "Temp en dehors [25;20]",
            MeasureType = SensorType.Temperature,
            RuleType = AlertRuleType.Outside,
            RangeMinValue = 25.0,
            RangeMaxValue = 20.0, // invalide car min > max
        };

        // When
        var repo = Substitute.For<IAlertRepository>();
        var usecase = new CreateAlertUsecase(repo, new AlertValidator());

        // Then
        await Assert.ThrowsAsync<ValidationException>(() => usecase.Handle(dto, userId, ct));
        await repo.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<Alert>(), CancellationToken.None);
    }

    [Fact]
    public async Task ShouldReturnAlertsByGreenhouse()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var greenhouseId = Guid.NewGuid();

        var repo = Substitute.For<IAlertRepository>();
        repo.GetByGreenhouseAsync(greenhouseId, ct)
            .Returns(
                [CreateAlert(AlertRuleType.Above, 10.0), CreateAlert(AlertRuleType.Below, 10.0)]
            );
        var ghRepo = Substitute.For<IGreenhouseRepository>();
        var mockGreenhouse = new Greenhouse
        {
            UserId = userId,
            Name = "GH",
            Country = "France",
            City = "Paris",
        };
        ghRepo.Exists(greenhouseId, userId, ct).Returns(mockGreenhouse);

        var usecase = new GetAlertsByGreenhouseUsecase(repo, ghRepo);

        // When
        var result = (await usecase.Handle(greenhouseId, userId, ct)).ToList();

        // Then
        await repo.Received(1).GetByGreenhouseAsync(greenhouseId, ct);
        await ghRepo.Received(1).Exists(greenhouseId, userId, ct);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ShouldThrowNotFound_WhenGreenhouseDoesNotExist()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var greenhouseId = Guid.NewGuid();

        var repo = Substitute.For<IAlertRepository>();
        var ghRepo = Substitute.For<IGreenhouseRepository>();
        ghRepo.Exists(greenhouseId, userId, ct).Returns((Greenhouse?)null);

        var usecase = new GetAlertsByGreenhouseUsecase(repo, ghRepo);

        // When
        var result = async () => await usecase.Handle(greenhouseId, userId, ct);

        // Then
        await Assert.ThrowsAsync<NotFoundEntityException>(result);
        await repo.DidNotReceiveWithAnyArgs()
            .GetByGreenhouseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await ghRepo.Received(1).Exists(greenhouseId, userId, ct);
    }

    [Fact]
    public async Task ShouldDeleteAlert_GivenValidId()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var alertId = Guid.NewGuid();

        var repo = Substitute.For<IAlertRepository>();
        repo.Exists(alertId, userId, ct).Returns(CreateAlert(AlertRuleType.Above, 10.0));
        repo.DeleteAsync(alertId, ct).Returns(true);

        var usecase = new DeleteAlertUsecase(repo);

        // When
        var result = await usecase.Handle(alertId, userId, ct);

        // Then
        await repo.Received(1).Exists(alertId, userId, ct);
        await repo.Received(1).DeleteAsync(alertId, ct);
        Assert.True(result);
    }

    [Fact]
    public async Task ShouldThrowNotFound_WhenAlertNotFound()
    {
        // Given
        var userId = "user_id123";
        var ct = CancellationToken.None;
        var alertId = Guid.NewGuid();

        var repo = Substitute.For<IAlertRepository>();
        repo.Exists(alertId, userId, ct).Returns((Alert?)null);

        var usecase = new DeleteAlertUsecase(repo);

        // When
        var result = async () => await usecase.Handle(alertId, userId, ct);

        // Then
        await Assert.ThrowsAsync<NotFoundEntityException>(result);
        await repo.Received(1).Exists(alertId, userId, ct);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), ct);
    }

    private Alert CreateAlert(
        AlertRuleType ruleType,
        double thresholdOrMin,
        double? max = null,
        bool enabled = true
    )
    {
        return new Alert
        {
            Name = "Test Alert",
            Enabled = enabled,
            RuleType = ruleType,
            MeasureType = SensorType.Temperature,
            ThresholdValue = ruleType is AlertRuleType.Above or AlertRuleType.Below
                ? thresholdOrMin
                : 0,
            RangeMinValue = ruleType is AlertRuleType.Inside or AlertRuleType.Outside
                ? thresholdOrMin
                : 0,
            RangeMaxValue = ruleType is AlertRuleType.Inside or AlertRuleType.Outside
                ? max ?? thresholdOrMin + 10
                : 0,
            GreenhouseId = Guid.NewGuid(),
        };
    }
}
