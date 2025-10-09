using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class OrderSupportTeamConfiguration : IEntityTypeConfiguration<OrderSupportTeam>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderSupportTeam> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.AdminId)
                .IsRequired();
                
            builder.HasOne(x => x.Admin)
                .WithMany()
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
