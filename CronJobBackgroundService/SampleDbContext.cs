using CronJobBackgroundService.Models;
using Microsoft.EntityFrameworkCore;

namespace CronJobBackgroundService;

public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasData(
            new Book { ID = 1, Name = "Book1" },
            new Book { ID = 2, Name = "Book2" },
            new Book { ID = 3, Name = "Book3" });
    }
}
