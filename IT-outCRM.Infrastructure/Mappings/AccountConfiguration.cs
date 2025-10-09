using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.FoundingDate)
                .IsRequired();
                
            builder.Property(x => x.AccountStatusId)
                .IsRequired();
                
            builder.HasOne(x => x.AccountStatus)
                .WithMany()
                .HasForeignKey(x => x.AccountStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
