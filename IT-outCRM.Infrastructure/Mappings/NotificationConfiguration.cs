using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
            builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
            builder.Property(n => n.Type).HasMaxLength(20).IsRequired();
            builder.Property(n => n.Link).HasMaxLength(500);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => new { n.UserId, n.IsRead });
            builder.HasIndex(n => n.CreatedAt);
        }
    }
}
