using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IT_outCRM.Infrastructure
{
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountStatus> AccountStatuses { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ContactPerson> ContactPersons { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Executor> Executors { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<OrderSupportTeam> OrderSupportTeams { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Deal> Deals { get; set; }

        public DbSet<DealMessage> DealMessages { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
