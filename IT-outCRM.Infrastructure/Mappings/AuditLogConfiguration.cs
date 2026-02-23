using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IT_outCRM.Infrastructure.Mappings
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityName).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Action).HasMaxLength(20).IsRequired();
            builder.Property(a => a.UserName).HasMaxLength(200);

            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => new { a.EntityName, a.EntityId });
        }
    }
}
