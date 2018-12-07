using System.Collections.Generic;

namespace data_rogue_core.Maps
{
    public class MapCollection : Dictionary<MapKey, Map>
    {
        public void AddMap(Map map)
        {
            this.Add(map.MapKey, map);
        }
    }
}