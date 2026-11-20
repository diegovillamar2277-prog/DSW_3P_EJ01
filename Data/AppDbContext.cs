using Microsoft.EntityFrameworkCore;
using QuestLog.Models;

namespace QuestLog.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Mision> Misiones => Set<Mision>();
}