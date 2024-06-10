using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Context;

public class SubscriptionContext : DbContext
{
    public DbSet<Client> Client { get; set; }
    public DbSet<Subscription> Subscription { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Discount> Discount { get; set; }
    public DbSet<Sale> Sale { get; set; }

    public SubscriptionContext()
    {
    }

    public SubscriptionContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasKey(c => c.IdClient);
        modelBuilder.Entity<Client>()
            .Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .Property(c => c.Phone)
            .HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Payments)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.IdClient);
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Sales)
            .WithOne(s => s.Client)
            .HasForeignKey(s => s.IdClient);
        modelBuilder.Entity<Subscription>()
            .HasKey(s => s.IdSubscription);
        modelBuilder.Entity<Subscription>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Subscription>()
            .Property(s => s.CreatedAt)
            .IsRequired();
        modelBuilder.Entity<Subscription>()
            .Property(s => s.RenewalPeriod)
            .IsRequired();
        modelBuilder.Entity<Subscription>()
            .Property(s => s.EndTime)
            .IsRequired();
        modelBuilder.Entity<Subscription>()
            .Property(s => s.Price)
            .IsRequired()
            .HasColumnType("money");
        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Payments)
            .WithOne(p => p.Subscription)
            .HasForeignKey(p => p.IdSubscription);
        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Discounts)
            .WithOne(d => d.Subscription)
            .HasForeignKey(d => d.IdSubscription);
        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Sales)
            .WithOne(s => s.Subscription)
            .HasForeignKey(s => s.IdSubscription);
        modelBuilder.Entity<Payment>()
            .HasKey(p => p.IdPayment);
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Payment>()
            .Property(p => p.Date)
            .IsRequired();

        modelBuilder.Entity<Discount>()
            .HasKey(d => d.IdDiscount);
        modelBuilder.Entity<Discount>()
            .Property(d => d.Value)
            .IsRequired();
        modelBuilder.Entity<Discount>()
            .Property(d => d.DateFrom)
            .IsRequired();
        modelBuilder.Entity<Discount>()
            .Property(d => d.DateTo)
            .IsRequired();

        modelBuilder.Entity<Sale>()
            .HasKey(s => s.IdSale);
        modelBuilder.Entity<Sale>()
            .Property(s => s.CreatedAt)
            .IsRequired();
    }
}