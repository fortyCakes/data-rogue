using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Maps.MapGenCommands
{
    public static class MapGenCommandExecutorFactory
    {
        public static List<ICommandExecutor> CommandExecutors =>
            AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommandExecutor).IsAssignableFrom(p) && p != typeof(ICommandExecutor) && !p.IsAbstract)
                .Select(type => (ICommandExecutor)Activator.CreateInstance(type))
                .ToList();

        public static ICommandExecutor GetExecutor(MapGenCommandType commandType)
        {
            return CommandExecutors.Single(s => s.CommandType == commandType);
        }
    }
}