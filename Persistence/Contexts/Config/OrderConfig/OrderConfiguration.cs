using System;
using Domain.Models.OrderModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Config.OrderConfig
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShipToAddress, a =>
            {
                a.WithOwner();
            });

            builder.Property(s => s.Status)
            .HasConversion(
                o => o.ToString(),
                o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
                );

            builder.HasMany(o => o.OrderItems)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}