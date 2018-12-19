using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        string GenerationType { get; }

        GeneratedBranch Generate(Branch branchDefinition, IEntityEngine engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed);
    }
}