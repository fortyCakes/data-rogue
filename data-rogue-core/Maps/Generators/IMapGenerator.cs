using data_rogue_core.Maps;

namespace data_rogue_core
{
    public interface IMapGenerator
    {
        IMap Generate(string mapName, IRandom random);
    }
}