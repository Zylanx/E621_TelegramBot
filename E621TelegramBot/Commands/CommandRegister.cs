using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace E621TelegramBot.Commands
{
    public static class CommandRegisterExtension
    {
        public static IServiceCollection AddBotCommands(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<CommandRegister>();

            return serviceCollection;
        }
    }

    public class CommandRegister
    {
        private readonly ILogger<CommandRegister> _logger;

        public List<IBotCommand> Commands { get; } = new();

        public CommandRegister(ILogger<CommandRegister> logger)
        {
            _logger = logger;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            _logger.LogInformation("Registering Commands");

            AppDomain.CurrentDomain.GetAssemblies()
                     .Select(assembly => assembly.GetTypes()
                                                 .Where(type => typeof(IBotCommand).IsAssignableFrom(type) &&
                                                                type.IsClass && !type.IsAbstract))
                     .ToList()
                     .ForEach(types =>
                     {
                         var typesList = types.ToList();
                         if (typesList.Count > 0)
                         {
                             typesList.ForEach(type => _logger.LogInformation($"Found Command: {type.Name}"));
                             Commands.AddRange(typesList.Select(type => (IBotCommand)Activator.CreateInstance(type)!));
                         }
                     });

            _logger.LogInformation("Finished Registering Commands");
        }
    }
}