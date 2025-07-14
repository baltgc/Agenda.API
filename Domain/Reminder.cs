using Agenda.API.Enums;

namespace Agenda.API.Domain;

public class Reminder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; }

    public DateTime ScheduledFor { get; set; }

    public ReminderStatus Status { get; set; }
}
