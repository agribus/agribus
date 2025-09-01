using Agribus.Application.AlertUsecases;
using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;
using Agribus.Core.Ports.Api.AlertUsecases.Validators;
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
