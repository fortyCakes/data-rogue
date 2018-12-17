using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        string GenerationType { get; }

        GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed);
    }
}