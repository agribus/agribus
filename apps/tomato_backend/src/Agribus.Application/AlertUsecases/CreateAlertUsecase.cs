using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;
using Agribus.Core.Ports.Api.AlertUsecases.Validators;
using Agribus.Core.Ports.Spi.AlertContext;
using FluentValidation;

namespace Agribus.Application.AlertUsecases;

public class CreateAlertUsecase(IAlertRepository alertRepository, AlertValidator validator)
    : ICreateAlertUsecase
{
    public async Task<Alert> Handle(CreateAlertDto dto, string userId, CancellationToken ct)
    {
        var entityToAdd = dto.MapToAlert();
        await validator.ValidateAndThrowAsync(entityToAdd, ct);
        var result = await alertRepository.AddAsync(entityToAdd, ct);
        return result;
    }
}
