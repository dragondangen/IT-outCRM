using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class DealConfiguration : IEntityTypeConfiguration<Deal>
    {
        public void Configure(EntityTypeBuilder<Deal> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AgreedPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Terms)
                .HasMaxLength(2000);

            builder.Property(x => x.CustomerReview)
                .HasMaxLength(1000);

            builder.Property(x => x.ExecutorReview)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Executor)
                .WithMany()
                .HasForeignKey(x => x.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Messages)
                .WithOne(m => m.Deal)
                .HasForeignKey(m => m.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.ExecutorId);
            builder.HasIndex(x => x.Status);
        }
    }
}
