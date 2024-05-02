namespace Krake.Core.Domain;

public abstract class DomainEvent(Guid? id = null, DateTime? occurredOnUtc = null) : IDomainEvent
{
    public Guid Id { get; } = id ?? Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = occurredOnUtc ?? DateTime.UtcNow;
}