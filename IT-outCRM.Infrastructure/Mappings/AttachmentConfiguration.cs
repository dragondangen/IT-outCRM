using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityType).HasMaxLength(50).IsRequired();
            builder.Property(a => a.FileName).HasMaxLength(500).IsRequired();
            builder.Property(a => a.StoredFileName).HasMaxLength(500).IsRequired();
            builder.Property(a => a.ContentType).HasMaxLength(100).IsRequired();

            builder.HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(a => new { a.EntityType, a.EntityId });
        }
    }
}
