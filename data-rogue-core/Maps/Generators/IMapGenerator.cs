using data_rogue_core.Maps;
using System;

namespace data_rogue_core
{
    public interface IMapGenerator
    {
        IMap Generate(string mapName, IRandom random, IProgress<string> progress);
    }
}