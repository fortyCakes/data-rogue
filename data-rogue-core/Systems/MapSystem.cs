using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Systems
{

    public class MapSystem : IMapSystem
    {
        private List<IMap> _vaults;

        public void Initialise()
        {
            MapCollection = new Atlas();
            _vaults = new List<IMap>();
        }

        public Atlas MapCollection { get; private set; }

        public IEnumerable<IMap> Vaults => _vaults;

        public IEntity CellAt(MapCoordinate coordinate) => MapCollection[coordinate.Key].CellAt(coordinate);

        public IMap TryGetMap(MapKey mapKey)
        {
            if (MapCollection.ContainsKey(mapKey))
            {
                return MapCollection[mapKey];
            }
            return null;
        }

        public void AddVault(IMap vault)
        {
            _vaults.Add(vault);
        }
    }
}