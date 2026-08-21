using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order.Domain.Entities.Order> Orders => Set<Order.Domain.Entities.Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order.Domain.Entities.Order>(builder =>
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.CustomerId).IsRequired();
            builder.Property(o => o.Status).HasConversion<string>();

            // آیتم‌های سفارش به‌صورت Owned Entity ذخیره می‌شوند
            builder.OwnsMany(o => o.Items, itemBuilder =>
            {
                itemBuilder.ToTable("OrderItems");
                itemBuilder.WithOwner().HasForeignKey("OrderId");
                itemBuilder.HasKey(i => i.Id);
            });

            builder.Metadata.FindNavigation(nameof(Order.Domain.Entities.Order.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
