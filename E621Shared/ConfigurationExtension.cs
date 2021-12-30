using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E621Shared
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddConfig<T>(this IServiceCollection services) where T : class
        {
            return services.AddTransient(provider =>
                provider.GetRequiredService<IConfiguration>().GetRequiredSection(typeof(T).Name).Get<T>());
        }

        public static IServiceCollection AddConfig<TConfig>(this IServiceCollection services,
                                                            TConfig config)
            where TConfig : class
        {
            return services.AddTransient(typeof(TConfig), _ => config);
        }
    }
}