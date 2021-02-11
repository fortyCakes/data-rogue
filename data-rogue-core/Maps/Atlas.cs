using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Maps
{
    public class Atlas : Dictionary<MapKey, IMap>
    {
        public void AddMap(IMap map)
        {
            Add(map.MapKey, map);
        }

        public List<IMap> AllMaps { get => Keys.Select(k => this[k]).ToList(); }
    }
}