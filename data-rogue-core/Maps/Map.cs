using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Maps
{
    public class Map
    {
        public MapKey MapKey { get; set; }

        public IEntity DefaultCell { get; set; }

        public Dictionary<MapCoordinate, IEntity> Cells = new Dictionary<MapCoordinate, IEntity>();

        public List<MapGenCommand> MapGenCommands { get; set; } = new List<MapGenCommand>();

        public uint DefaultCellId
        {
            get
            {
                return DefaultCell.EntityId;
            }
        }

        public int LeftX => Cells.Any() ? Cells.Min(c => c.Key.X) : 0;
        public int TopY => Cells.Any() ? Cells.Min(c => c.Key.Y) : 0;
        public int RightX => Cells.Any() ? Cells.Max(c => c.Key.X) : 0;
        public int BottomY => Cells.Any() ? Cells.Max(c => c.Key.Y) : 0;
        public Vector Origin => new Vector(LeftX, TopY);

        public Map(string key, IEntity defaultCell)
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

        public bool CellExists(int x, int y)
        {
            return Cells.ContainsKey(new MapCoordinate(MapKey, x, y));
        }

        public void SetCell(int x, int y, IEntity cell)
        {
            SetCell(new MapCoordinate(MapKey, x, y), cell);
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

        public IEntity CellAt(MapCoordinate coordinate)
        {
            if (coordinate.Key != MapKey) return null;

            if (Cells.ContainsKey(coordinate))
            {
                return Cells[coordinate];
            }
            else
            {
                return DefaultCell;
            }
        }

        public void SetCell(MapCoordinate coordinate, IEntity cell)
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

        public void RemoveCellsInRange(int x1, int x2, int y1, int y2)
        {
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            {
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                {
                    RemoveCell(x, y);
                }
            }
        }

        public void RemoveCell(MapCoordinate mapCoordinate)
        {
            if (Cells.ContainsKey(mapCoordinate))
            {
                Cells.Remove(mapCoordinate);
            }
        }

        public void RemoveCell(int x, int y)
        {
            RemoveCell(new MapCoordinate(MapKey, x, y));
        }

        public List<MapCoordinate> FovFrom(MapCoordinate mapCoordinate, int range)
        {
            if (mapCoordinate.Key != this.MapKey)
            {
                return new List<MapCoordinate>();
            }

            bool GetTransparent(Vector v) => CellAt(mapCoordinate + v).Get<Physical>().Transparent;

            var visibleVectors = ShadowcastingFovCalculator.InFov(range, GetTransparent);

            var visibleCells = visibleVectors.Select(v => mapCoordinate + v).ToList();

            return visibleCells;
        }

        public void ClearCell(MapCoordinate coordinate)
        {
            if (Cells.ContainsKey(coordinate))
            {
                Cells.Remove(coordinate);
            }
        }
    }
}