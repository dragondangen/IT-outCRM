using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Duration)
                .IsRequired();

            builder.Property(x => x.Category)
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExecutorId)
                .IsRequired();

            builder.HasOne(x => x.Executor)
                .WithMany()
                .HasForeignKey(x => x.ExecutorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ExecutorId);
            builder.HasIndex(x => x.IsActive);
        }
    }
}

