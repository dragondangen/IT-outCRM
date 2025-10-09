using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.Description)
                .HasMaxLength(1000);
                
            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);
                
            builder.Property(x => x.CustomerId)
                .IsRequired();
                
            builder.Property(x => x.ExecutorId)
                .IsRequired();
                
            builder.Property(x => x.OrderStatusId)
                .IsRequired();
                
            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.Executor)
                .WithMany()
                .HasForeignKey(x => x.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.OrderStatus)
                .WithMany()
                .HasForeignKey(x => x.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
