﻿using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Maps
{
    public class MapCollection : Dictionary<MapKey, Map>
    {
        public void AddMap(Map map)
        {
            Add(map.MapKey, map);
        }

        public List<Map> AllMaps { get => Keys.Select(k => this[k]).ToList(); }
    }
}