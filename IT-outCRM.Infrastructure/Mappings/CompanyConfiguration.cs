using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.Inn)
                .IsRequired()
                .HasMaxLength(12);
                
            builder.Property(x => x.LegalForm)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.HasIndex(x => x.Inn)
                .IsUnique();

            builder.HasOne(x => x.ContactPerson)
                .WithMany()
                .HasForeignKey(x => x.ContactPersonID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
