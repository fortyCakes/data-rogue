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

        public Dictionary<MapCoordinate, IEntity> Cells = new Dictionary<MapCoordinate, IEntity>();

        public Map(string key, Entity defaultCell)
        {
            CheckEntityIsCell(defaultCell);

            MapKey = new MapKey(key);
            DefaultCell = defaultCell;
        }

        private static void CheckEntityIsCell(IEntity defaultCell)
        {
            if (!defaultCell.Has<Physical>())
            {
                throw new ArgumentException("Map cells must be Physical");
            }

            if (!defaultCell.Has<Appearance>())
            {
                throw new ArgumentException("Map cells must have Appearance");
            }
        }

        public IEntity CellAt(int lookupX, int lookupY)
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

        internal void SetCell(MapCoordinate coordinate, IEntity cell)
        {
            CheckEntityIsCell(cell);

            Cells[coordinate] = cell;
        }

        public void SetCellsInRange(int x1, int x2, int y1, int y2, IEntity cell)
        {
            CheckEntityIsCell(cell);

            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            {
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                {
                    SetCell(new MapCoordinate(MapKey, x, y), cell);
                }
            }
        }
    }
}