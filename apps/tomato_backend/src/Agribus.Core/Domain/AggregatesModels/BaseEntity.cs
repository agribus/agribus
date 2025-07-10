using System.ComponentModel.DataAnnotations.Schema;

namespace Agribus.Core.Domain.AggregatesModels;

public class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private init; } = Guid.NewGuid();

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.UtcNow;

    public void UpdateLastModified()
    {
        LastModified = DateTimeOffset.UtcNow;
    }
}
