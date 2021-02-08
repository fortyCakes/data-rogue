using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Controls.MapEditorTools
{
    public interface IMapEditorTool
    {
        IEntity Entity { get; }
        bool RequiresClick { get; }

        void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell);
    }
}