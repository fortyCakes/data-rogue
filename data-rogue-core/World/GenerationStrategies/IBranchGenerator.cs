using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core
{
    public interface IBranchGenerator
    {
        GeneratedBranch Generate(ISystemContainer systemContainer, IEntity branchEntity, IProgress<string> progress);
    }
}