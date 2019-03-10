using data_rogue_core.World.GenerationStrategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public static class MapGeneratorFactory
    {
        public static List<IBranchGenerator> BranchGenerators =>
            AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IBranchGenerator).IsAssignableFrom(p) && p != typeof(IBranchGenerator) && !p.IsAbstract )
                .Select(type => (IBranchGenerator)Activator.CreateInstance(type))
                .ToList();

        public static IBranchGenerator GetGenerator(string branchDefinitionGenerationType)
        {
            return BranchGenerators.Single(s => s.GenerationType == branchDefinitionGenerationType);
        }
    }
}