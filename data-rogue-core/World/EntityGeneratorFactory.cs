using data_rogue_core.World.GenerationStrategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{

    public static class EntityGeneratorFactory
    {
        public static List<IEntityGenerator> EntityGenerators =>
            AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IEntityGenerator).IsAssignableFrom(p) && p != typeof(IEntityGenerator) && !p.IsAbstract)
                .Select(type => (IEntityGenerator)Activator.CreateInstance(type))
                .ToList();

        public static IEntityGenerator GetGenerator(string entityGenerationType)
        {
            return EntityGenerators.SingleOrDefault(s => s.GenerationType == entityGenerationType);
        }
    }
}