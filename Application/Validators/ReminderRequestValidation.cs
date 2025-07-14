using Agenda.API.API;
using FluentValidation;

public class ReminderRequestValidator : AbstractValidator<ReminderRequest>
{
    public ReminderRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ScheduledFor).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}
