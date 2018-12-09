using data_rogue_core.Data;
using data_rogue_core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public class MapCollection : Dictionary<MapKey, Map>
    {
        public void AddMap(Map map)
        {
            this.Add(map.MapKey, map);
        }

        public List<Map> AllMaps { get => Keys.Select(k => this[k]).ToList(); }
    }
}