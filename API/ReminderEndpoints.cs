using System.Security.Claims;
using Agenda.API.Application;
using Agenda.API.Domain;
using Agenda.API.Enums;

namespace Agenda.API.API;

public static class ReminderEndpoints
{
    public static void MapReminderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reminders").RequireAuthorization();

        group.MapGet(
            "/",
            async (ReminderService svc, ClaimsPrincipal user) =>
            {
                var reminders = await svc.GetAllAsync(user);
                return Results.Ok(reminders);
            }
        );

        group.MapGet(
            "/{id:int}",
            async (int id, ReminderService svc, ClaimsPrincipal user) =>
            {
                var reminder = await svc.GetByIdAsync(id, user);
                return Results.Ok(reminder);
            }
        );

        group.MapPost(
            "/",
            async (ReminderRequest req, ReminderService svc, ClaimsPrincipal user) =>
            {
                var reminder = new Reminder
                {
                    Title = req.Title,
                    Description = req.Description,
                    ScheduledFor = req.ScheduledFor,
                    Status = ReminderStatus.Active,
                };

                var created = await svc.CreateAsync(reminder, user);
                return Results.Created($"/reminders/{created.Id}", created);
            }
        );

        group.MapPut(
            "/{id:int}",
            async (int id, ReminderRequest req, ReminderService svc, ClaimsPrincipal user) =>
            {
                var updated = new Reminder
                {
                    Title = req.Title,
                    Description = req.Description,
                    ScheduledFor = req.ScheduledFor,
                    Status = req.Status,
                };

                await svc.UpdateAsync(id, updated, user);
                return Results.Ok();
            }
        );

        group.MapDelete(
            "/{id:int}",
            async (int id, ReminderService svc, ClaimsPrincipal user) =>
            {
                await svc.DeleteAsync(id, user);
                return Results.NoContent();
            }
        );
    }
}

public record ReminderRequest(
    string Title,
    string Description,
    DateTime ScheduledFor,
    ReminderStatus Status = ReminderStatus.Active
);
