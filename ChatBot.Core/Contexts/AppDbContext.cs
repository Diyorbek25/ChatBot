using ChatBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Core.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
    }
}
