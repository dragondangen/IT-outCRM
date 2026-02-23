using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Infrastructure.Interceptors;
using IT_outCRM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IT_outCRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddScoped<AuditSaveChangesInterceptor>();

            services.AddDbContext<CrmDbContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                var interceptor = sp.GetService<AuditSaveChangesInterceptor>();
                if (interceptor != null)
                    options.AddInterceptors(interceptor);
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountStatusRepository, AccountStatusRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IExecutorRepository, ExecutorRepository>();
            services.AddScoped<IContactPersonRepository, ContactPersonRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IDealRepository, DealRepository>();
            services.AddScoped<IDealMessageRepository, DealMessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}

