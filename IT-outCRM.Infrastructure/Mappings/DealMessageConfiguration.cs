using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class DealMessageConfiguration : IEntityTypeConfiguration<DealMessage>
    {
        public void Configure(EntityTypeBuilder<DealMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SenderName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SenderRole)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.DealId);
        }
    }
}
