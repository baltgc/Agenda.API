using System.Security.Claims;
using Agenda.API.Domain;
using Agenda.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Agenda.API.Application;

public class ReminderService
{
    private readonly AgendaDbContext _db;

    public ReminderService(AgendaDbContext db)
    {
        _db = db;
    }

    public async Task<List<Reminder>> GetAllAsync(ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        return await _db.Reminders.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<Reminder> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        var reminder = await _db.Reminders.FirstOrDefaultAsync(r =>
            r.Id == id && r.UserId == userId
        );

        return reminder ?? throw new Exception("Reminder not found");
    }

    public async Task<Reminder> CreateAsync(Reminder reminder, ClaimsPrincipal user)
    {
        reminder.UserId = GetUserId(user);
        _db.Reminders.Add(reminder);
        await _db.SaveChangesAsync();
        return reminder;
    }

    public async Task UpdateAsync(int id, Reminder updated, ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        var reminder =
            await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId)
            ?? throw new Exception("Reminder not found");

        reminder.Title = updated.Title;
        reminder.Description = updated.Description;
        reminder.ScheduledFor = updated.ScheduledFor;
        reminder.Status = updated.Status;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        var reminder =
            await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId)
            ?? throw new Exception("Reminder not found");

        _db.Reminders.Remove(reminder);
        await _db.SaveChangesAsync();
    }

    private int GetUserId(ClaimsPrincipal user)
    {
        return int.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Invalid JWT: NameIdentifier claim missing")
        );
    }
}
