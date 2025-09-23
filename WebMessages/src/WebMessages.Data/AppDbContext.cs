using Microsoft.EntityFrameworkCore;
using WebMessages.Models.Entities;

namespace WebMessages.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índice único no Username
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Relação Message -> Device (FromDevice)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.FromDevice)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.FromDeviceId)
            .OnDelete(DeleteBehavior.Restrict); // evita cascade delete

        // Relação Message -> Device (ToDevice)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.ToDevice)
            .WithMany(u => u.MessagesReceived)
            .HasForeignKey(m => m.ToDeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relação Device -> User
       modelBuilder.Entity<Device>()
            .HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
