using data_rogue_core.Maps;

namespace data_rogue_core.Activities
{
    public interface IMapActivity
    {
        MapCoordinate CameraPosition { get; }
    }
}