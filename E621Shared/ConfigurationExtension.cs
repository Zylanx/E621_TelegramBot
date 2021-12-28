using Microsoft.Extensions.Configuration;

namespace E621Shared
{
    public static class ConfigurationExtension
    {
        public static T BindSection<T>(this IConfiguration config) where T : new()
        {
            return config.GetRequiredSection(typeof(T).Name).Get<T>();
        }
    }
}