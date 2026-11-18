using Microsoft.EntityFrameworkCore;
using ClaimsWebApi.Data;
using ClaimsWebApi.Exceptions;
using ClaimsWebApi.Servicies;
using ClaimsWebApi.Servicies.Imp;

namespace ClaimsWebApi
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddDbContext<ClaimsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            RegisterInstances(services);
            
            return services;
        }

        private static void RegisterInstances(IServiceCollection services)
        {
            services.AddScoped<IClaimService, ClaimService>();
        }
    }
}
