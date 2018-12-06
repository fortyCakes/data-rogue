using data_rogue_core.Extensions;

namespace data_rogue_core.Data
{
    public class MapCollection : SDictionary<MapKey, Map>
    {
        public void AddMap(Map map)
        {
            this.Add(map.MapKey, map);
        }
    }
}