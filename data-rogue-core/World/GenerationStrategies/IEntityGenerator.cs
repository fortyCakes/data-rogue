using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.World.GenerationStrategies
{
    public interface IEntityGenerator
    {
        void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress);
    }
}
