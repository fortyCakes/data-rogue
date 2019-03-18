using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IMapSystem
    {
        MapCollection MapCollection { get; }

        void Initialise();
    }
}
