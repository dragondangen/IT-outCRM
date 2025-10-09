using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Mappings
{
    internal class ExecutorConfiguration : IEntityTypeConfiguration<Executor>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Executor> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.CompletedOrders)
                .IsRequired();
                
            builder.Property(x => x.AccountId)
                .IsRequired();
                
            builder.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
