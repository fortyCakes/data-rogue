using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        string GenerationType { get; }

        GeneratedBranch Generate(ISystemContainer systemContainer, Branch branchDefinition);
    }
}