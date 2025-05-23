using LPRStoresAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LPRStoresAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Inherit from IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        // ApplicationUser DbSet is inherited from IdentityDbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity tables

            // Configure Customer Email as unique
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Configure Product Name as unique (optional, but good for this system)
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .IsUnique();
                
            // Define relationships (EF Core might infer some, but explicit is better)

            // Customer to Orders (One-to-Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on requirements

            // Order to OrderItems (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Product to OrderItems (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);
                
            // Configure decimal precision for relevant properties (already done with attributes, but can be centralized here)
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
}
