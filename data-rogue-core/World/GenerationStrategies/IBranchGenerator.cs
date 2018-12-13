using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Systems;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        string GenerationType { get; }

        GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem positionSystem, string seed);
    }
}