using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace E621TelegramBot.Commands
{
    public static class CommandRegisterExtension
    {
        public static IServiceCollection AddBotCommands(this IServiceCollection serviceCollection)
        {
            AppDomain.CurrentDomain.GetAssemblies()
                     .Select(assembly => assembly.GetTypes()
                                                 .Where(type => typeof(IBotCommand).IsAssignableFrom(type) &&
                                                                type.IsClass && !type.IsAbstract))
                     .ToList()
                     .ForEach(commands => commands.ToList().ForEach(type =>
                     {
                         serviceCollection.AddTransient(typeof(IBotCommand), type);
                     }));

            return serviceCollection;
        }
    }
}