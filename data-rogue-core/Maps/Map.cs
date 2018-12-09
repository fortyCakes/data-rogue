using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using System.Linq;
using System.Runtime.Serialization;
using data_rogue_core.Data;

namespace data_rogue_core.Maps
{
    public class Map : ISerializable
    {
        public MapKey MapKey { get; set; }

        public IEntity DefaultCell { get; private set; }

        public uint DefaultCellId
        {
            get
            {
                return DefaultCell.EntityId;
            }
        }

        public Dictionary<MapCoordinate, IEntity> Cells = new Dictionary<MapCoordinate, IEntity>();

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

        public SaveMap Serialize()
        {
            return new SaveMap
            {
                MapKey = MapKey.Key,
                Cells = Cells.Keys.Select(k => new MapSaveCell
                {
                    X = k.X,
                    Y = k.Y,
                    Id = Cells[k].EntityId
                }).ToList()
            };
        }

        public static Map Deserialize(SaveMap savedMap, IEntityEngineSystem entityEngineSystem)
        {
            var key = savedMap.MapKey;
            var defaultCell = entityEngineSystem.GetEntity(savedMap.DefaultCellId);

            var map = new Map(key, defaultCell);
            
            foreach(var savedCell in savedMap.Cells)
            {
                var cell = entityEngineSystem.GetEntity(savedCell.Id);
                map.SetCell(new MapCoordinate(key, savedCell.X, savedCell.Y), cell);
            }

            return map;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}