using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace E621TelegramBot.Commands
{
    // TODO: Add a command register which maps command text to commands and which handles command conflicts


    public static class CommandRegistrationExtension
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
                         if (type != typeof(Commands.System.Help))
                         {
                             serviceCollection.AddTransient(typeof(IBotCommand), type);
                         }
                     }));

            return serviceCollection;
        }
    }
}