using FluentValidation;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IT_outCRM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Services
            services.AddScoped<IEntityValidationService, EntityValidationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountStatusService, AccountStatusService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IExecutorService, ExecutorService>();
            services.AddScoped<IContactPersonService, ContactPersonService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}

