using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Data
{
    [Serializable]
    public class Map
    {
        public MapKey MapKey { get; private set; }
        public Entity DefaultCell { get; private set; }

        public Dictionary<MapCoordinate, Entity> Cells = new Dictionary<MapCoordinate, Entity>();

        public Map(string key, Entity defaultCell)
        {
            CheckEntityIsCell(defaultCell);

            MapKey = new MapKey(key);
            DefaultCell = defaultCell;
        }

        private static void CheckEntityIsCell(Entity defaultCell)
        {
            if (!defaultCell.Has<Terrain>())
            {
                throw new ArgumentException("Map cells must have Terrain");
            }

            if (!defaultCell.Has<Appearance>())
            {
                throw new ArgumentException("Map cells must have Appearance");
            }
        }

        public Entity CellAt(int lookupX, int lookupY)
        {
            var coordinate = new MapCoordinate(MapKey, lookupX, lookupY);
            if (Cells.ContainsKey(coordinate))
            {
                return Cells[coordinate];
            }
            else
            {
                return DefaultCell;
            }
        }

        internal void SetCell(MapCoordinate coordinate, Entity cell)
        {
            CheckEntityIsCell(cell);

            Cells[coordinate] = cell;
        }
    }
}