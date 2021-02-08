using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public interface IMapEditorTool
    {
        IEntity Entity { get; }
        bool RequiresClick { get; }

        IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate);

        void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, IActivitySystem activitySystem);
    }
}