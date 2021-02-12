using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Utils;

namespace data_rogue_core.Maps
{
    public class Map : IMap, ICloneable
    {
        public MapKey MapKey { get; set; }

        public IEntity DefaultCell { get; set; }

        public Dictionary<MapCoordinate, IEntity> Cells { get; set; } = new Dictionary<MapCoordinate, IEntity>();

        public HashSet<MapCoordinate> SeenCoordinates { get; set; } = new HashSet<MapCoordinate>();

        public List<MapGenCommand> MapGenCommands { get; set; } = new List<MapGenCommand>();

        public IEnumerable<MapKey> Vaults { get; set; } = new List<MapKey>();

        public IEnumerable<Biome> Biomes { get; set; } = new List<Biome>();

        private IFovCache FovCache;

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
            FovCache = new FovCache();
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

        public bool CellExists(MapCoordinate coordinate)
        {
            return Cells.ContainsKey(coordinate);
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

            InvalidateCache();
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
                InvalidateCache();
            }
        }

        public void RemoveCell(int x, int y)
        {
            RemoveCell(new MapCoordinate(MapKey, x, y));
        }

        public List<MapCoordinate> FovFrom(IPositionSystem positionSystem, MapCoordinate mapCoordinate, int range, Func<Vector, bool> transparentTest = null)
        {
            var cachedFov = FovCache.TryGetCachedFov(mapCoordinate, range);

            if (cachedFov != null)
            {
                return cachedFov;
            }

            if (mapCoordinate.Key != MapKey)
            {
                return new List<MapCoordinate>();
            }

            if (transparentTest == null)
            {
                transparentTest = (Vector v) =>
                {
                    var entities = positionSystem.EntitiesAt(mapCoordinate + v);
                    var physicals = entities.Select(e => e.TryGet<Physical>()).Where(p => p != null);
                    return !physicals.Any(p => p.Transparent == false);
                };
            }

            var visibleVectors = ShadowcastingFovCalculator.InFov(range, transparentTest);

            var visibleCells = visibleVectors.Select(v => mapCoordinate + v).ToList();

            FovCache.Cache(mapCoordinate, range, visibleCells);

            return visibleCells;
        }

        public void ClearCell(MapCoordinate coordinate)
        {
            if (Cells.ContainsKey(coordinate))
            {
                Cells.Remove(coordinate);

                InvalidateCache();
            }
        }

        public void SetSeen(MapCoordinate coordinate, bool seen = true)
        {
            SeenCoordinates.Add(coordinate);
        }

        public void SetSeen(int x, int y, bool seen = true)
        {
            SeenCoordinates.Add(new MapCoordinate(MapKey, x, y));
        }

        public void InvalidateCache()
        {
            FovCache.Invalidate();
        }

        public void AddCommand(MapGenCommand command)
        {
            MapGenCommands.Add(command);
        }

        public void RemoveCommandsAt(int x, int y)
        {
            Vector vector = new Vector(x, y);
            var commandsToRemove = MapGenCommands.Where(m => m.Vector == vector).ToList();

            foreach(var command in commandsToRemove)
            {
                MapGenCommands.Remove(command);
            }
        }

        public void RemoveCommandsAt(MapCoordinate mapCoordinate)
        {
            if (mapCoordinate.Key == MapKey)
            {
                RemoveCommandsAt(mapCoordinate.X, mapCoordinate.Y);
            }
        }

        public bool HasCommandAt(int x, int y)
        {
            Vector vector = new Vector(x, y);
            return MapGenCommands.Any(m => m.Vector == vector);
        }

        public bool HasCommandAt(MapCoordinate mapCoordinate)
        {
            if (mapCoordinate.Key == MapKey)
            {
                return HasCommandAt(mapCoordinate.X, mapCoordinate.Y);
            }

            throw new ApplicationException("MapCoordinate is not for this map.");
        }

        public IEnumerable<IEnumerable<MapCoordinate>> GetSections()
        {
            var uncheckedCells = new List<MapCoordinate>(Cells.Keys);
            var sections = new List<IEnumerable<MapCoordinate>>();

            while(uncheckedCells.Any())
            {
                var startCell = uncheckedCells.First();

                var section = FloodFillHelper.FloodFill(startCell, CellExists, 100000);

                sections.Add(section);

                foreach(var cellToRemove in section)
                {
                    uncheckedCells.Remove(cellToRemove);
                }
            }

            return sections;
        }

        public object Clone()
        {
            var clone = new Map(MapKey.Key, DefaultCell);

            clone.Cells = new Dictionary<MapCoordinate, IEntity>(Cells);
            clone.SeenCoordinates = new HashSet<MapCoordinate>(SeenCoordinates);
            clone.MapGenCommands = new List<MapGenCommand>(MapGenCommands.Select(c => (MapGenCommand)c.Clone()));
            clone.Vaults = new List<MapKey>(Vaults);
            clone.Biomes = new List<Biome>(Biomes);

            return clone;
        }

        public void Spin(IRandom random)
        {
            var randomRotation = random.PickOneFrom(Matrix.Identity, Matrix.Rotate90, Matrix.Rotate180, Matrix.Rotate270);

            Rotate(randomRotation);
        }

        public void Rotate(Matrix matrix)
        {
            var newLocations = new Dictionary<MapCoordinate, IEntity>();

            foreach (var cell in Cells)
            {
                var newCoordinate = new MapCoordinate(MapKey, matrix * cell.Key.ToVector());

                newLocations.Add(newCoordinate, cell.Value);
            }

            foreach (var mapGenCommand in MapGenCommands)
            {
                mapGenCommand.Vector = matrix * mapGenCommand.Vector;
            }

            Cells = newLocations;
        }

        public void PlaceSubMap(MapCoordinate position, IMap selectedVault)
        {
            position = position - selectedVault.Origin;

            foreach(var cell in selectedVault.Cells)
            {
                SetCell(position + cell.Key.ToVector(), cell.Value);
            }

            foreach(var mapCommand in selectedVault.MapGenCommands)
            {
                var newMapCommand = new MapGenCommand
                {
                    MapGenCommandType = mapCommand.MapGenCommandType,
                    Vector = mapCommand.Vector + position.ToVector(),
                    Parameters = mapCommand.Parameters
                };
                MapGenCommands.Add(newMapCommand);
            }
        }

        public Size GetSize()
        {
            return new Size(RightX - LeftX, BottomY - TopY);
        }

        public bool IsLocationEmpty(MapCoordinate coordinate, Size size)
        {
            for(int x = coordinate.X; x <= coordinate.X + size.Width; x++)
            {
                for (int y = coordinate.Y; y <= coordinate.Y + size.Height; y++)
                {
                    if (CellExists(x, y)) return false;
                }
            }

            return true;
        }
    }
}