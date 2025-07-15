namespace Agenda.API.Domain;

public interface IDomainEvent
{
    DateTime OccuredAt { get; }
}

public class ReminderCreatedEvent : IDomainEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;

    public string UserId { get; }

    public string Title { get; }

    public DateTime ScheduledFor { get; }

    public ReminderCreatedEvent(string userId, string title, DateTime scheduledFor)
    {
        UserId = userId;
        Title = title;
        ScheduledFor = scheduledFor;
    }
}
