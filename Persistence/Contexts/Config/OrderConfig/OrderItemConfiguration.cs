using Domain.Models.OrderModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Config.OrderConfig
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered, io =>
            {
                io.WithOwner();
            });

            builder.Property(i => i.Price)
            .HasColumnType("decimal(18,2)");
        }
    }
}