using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.World.GenerationStrategies;
using System;

namespace data_rogue_core.Components
{
    public abstract class BaseEntityGenerationStrategy : IEntityComponent, IEntityGenerator
    {
        public int BasePower = 0;
        public int PowerIncrement = 1;
        public decimal Density;

        public abstract void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress);
    }
}
