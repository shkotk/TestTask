using Microsoft.EntityFrameworkCore;
using TestTask.UserApi.Models;

namespace TestTask.UserApi;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions options) 
        : base(options)
    {
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.HasOne<Subscription>()
                .WithOne()
                .HasForeignKey<User>(e => e.SubscriptionId);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate);
        });
    }
}