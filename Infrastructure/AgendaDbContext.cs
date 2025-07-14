using Agenda.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.API.Infrastructure;

public class AgendaDbContext : DbContext
{
    public AgendaDbContext(DbContextOptions<AgendaDbContext> options)
        : base(options) { }

    public DbSet<Reminder> Reminders => Set<Reminder>();
}
