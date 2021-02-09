using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public abstract class BaseFilledShapeTool : BaseShapeTool
    {
        public override void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, ISystemContainer systemContainer)
        {
            if (FirstCoordinate == null)
            {
                FirstCoordinate = mapCoordinate;

            }
            else
            {
                var fill = GetInternalAffected(FirstCoordinate, mapCoordinate);

                foreach (var coordinate in fill)
                {
                    map.SetCell(coordinate, alternateCell);
                }

                base.Apply(map, mapCoordinate, currentCell, alternateCell, systemContainer);
            }
        }

        protected abstract IEnumerable<MapCoordinate> GetInternalAffected(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate);

        public override IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate)
        {
            if (FirstCoordinate == null)
            {
                return new List<MapCoordinate>();
            }
            else
            {
                return GetInternalAffected(FirstCoordinate, secondCoordinate);
            }
        
        }
    }
}