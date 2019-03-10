using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.World.GenerationStrategies
{
    public interface IEntityGenerator
    {
        string GenerationType { get; }

        void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, EntityGenerationStrategy step, IRandom random);
    }
}
