using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public abstract class BaseShapeTool : IMapEditorTool
    {
        public MapCoordinate FirstCoordinate;
        public abstract IEntity Entity { get; }

        public bool RequiresClick => true;

        public virtual void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, ISystemContainer systemContainer)
        {
            if (FirstCoordinate == null)
            {
                FirstCoordinate = mapCoordinate;

            }
            else
            {
                var coordinates = GetAffectedCoordinates(FirstCoordinate, mapCoordinate);

                foreach (var coordinate in coordinates)
                {
                    map.SetCell(coordinate, currentCell);
                }

                FirstCoordinate = null;
            }
        }

        protected abstract IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate);

        public virtual IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate) => new List<MapCoordinate>();

        protected static IEnumerable<MapCoordinate> HorizontalLine(MapKey key, int minX, int maxX, int y)
        {
            for (int x = minX; x <= maxX; x++)
            {
                yield return new MapCoordinate(key, x, y);
            }
        }

        protected static IEnumerable<MapCoordinate> VerticalLine(MapKey key, int x, int minY, int maxY)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return new MapCoordinate(key, x, y);
            }
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            if (FirstCoordinate == null)
            {
                return new List<MapCoordinate> { mapCoordinate };
            }
            else
            {
                return GetAffectedCoordinates(FirstCoordinate, mapCoordinate);
            }
        }

    }
}