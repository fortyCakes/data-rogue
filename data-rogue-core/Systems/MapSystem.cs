using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Systems
{

    public class MapSystem : IMapSystem
    {
        public void Initialise()
        {
            MapCollection = new MapCollection();
            Vaults = new List<IMap>();
        }

        public MapCollection MapCollection { get; private set; }

        public IEnumerable<IMap> Vaults { get; private set; }

        public IEntity CellAt(MapCoordinate coordinate) => MapCollection[coordinate.Key].CellAt(coordinate);

        public IMap TryGetMap(MapKey mapKey)
        {
            if (MapCollection.ContainsKey(mapKey))
            {
                return MapCollection[mapKey];
            }
            return null;
        }
    }
}