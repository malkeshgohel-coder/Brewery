using Brewery.Repositories.Repositories;
using Brewery.Repositories.Repositories.IRepositories;
using Brewery.Services.Service;
using Brewery.Services.Service.IService;

namespace Brewery.ServiceCollection
{
    public static class ServiceCollection
    {
        public static IServiceCollection InjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<IBreweryRepository, InMemoryBreweryRepository>();
            services.AddScoped<IOpenBreweryRepository, OpenBreweryRepository>();
            services.AddScoped<IBreweryService, BreweryService>();

            services.AddLazyResolution();
            return services;
        }
        public static IServiceCollection AddLazyResolution(this IServiceCollection services)
        {
            return services.AddTransient(
                typeof(Lazy<>),
                typeof(LazilyResolved<>));
        }

        private class LazilyResolved<T> : Lazy<T>
        {
            public LazilyResolved(IServiceProvider serviceProvider)
                : base(serviceProvider.GetRequiredService<T>)
            {
            }
        }
    }
}
