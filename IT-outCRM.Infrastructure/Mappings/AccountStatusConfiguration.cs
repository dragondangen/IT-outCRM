using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class AccountStatusConfiguration : IEntityTypeConfiguration<AccountStatus>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AccountStatus> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
