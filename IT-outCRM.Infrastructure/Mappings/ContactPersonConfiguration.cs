using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class ContactPersonConfiguration : IEntityTypeConfiguration<ContactPerson>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ContactPerson> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.SurName)
                .HasMaxLength(100);
                
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
