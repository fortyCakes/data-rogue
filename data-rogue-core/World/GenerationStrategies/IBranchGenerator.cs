using data_rogue_core.Components;
using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        string GenerationType { get; }

        GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, string seed);
    }
}