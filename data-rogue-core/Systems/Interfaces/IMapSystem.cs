using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IMapSystem
    {
        MapCollection MapCollection { get; }

        IEntity CellAt(MapCoordinate coordinate);

        void Initialise();
    }
}
