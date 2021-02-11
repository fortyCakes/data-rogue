using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System.Collections.Generic;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IMapSystem
    {
        Atlas MapCollection { get; }
        IEnumerable<IMap> Vaults { get; }

        IEntity CellAt(MapCoordinate coordinate);

        void Initialise();
        IMap TryGetMap(MapKey key);
        void AddVault(IMap vault);
    }
}
