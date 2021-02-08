using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public abstract class BaseFilledShapeTool : BaseShapeTool
    {
        public override void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell)
        {
            if (FirstCoordinate == null)
            {
                FirstCoordinate = mapCoordinate;

            }
            else
            {
                var fill = GetInternalCoordinates(FirstCoordinate, mapCoordinate);

                foreach (var coordinate in fill)
                {
                    map.SetCell(coordinate, alternateCell);
                }

                base.Apply(map, mapCoordinate, currentCell, alternateCell);
            }
        }

        protected abstract IEnumerable<MapCoordinate> GetInternalCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate);
    }
}