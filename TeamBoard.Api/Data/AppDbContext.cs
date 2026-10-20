using Microsoft.EntityFrameworkCore;
using TeamBoard.Api.Entities;

namespace TeamBoard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(e =>
        {
            e.Property(b => b.Name).HasMaxLength(200).IsRequired();
            e.Property(b => b.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("Tasks");
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.Property(t => t.Description).HasMaxLength(2000);
            e.Property(t => t.Assignee).HasMaxLength(100);
            e.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            e.HasOne(t => t.Board)
                .WithMany(b => b.Tasks)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
