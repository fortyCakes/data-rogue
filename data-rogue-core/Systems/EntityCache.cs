using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public class EntityCache : Dictionary<MapCoordinate, List<IEntity>>
    {
        public void Add(MapCoordinate mapCoordinate, IEntity entity)
        {
            if (ContainsKey(mapCoordinate))
            {
                this[mapCoordinate].Add(entity);
            }
            else
            {
                Add(mapCoordinate, new List<IEntity> { entity });
            }
        }

        public void Remove(IEntity entity)
        {
            var keysToRemove = new List<MapCoordinate>();

            foreach (var kvp in this)
            {
                if (kvp.Value.Contains(entity))
                {
                    kvp.Value.Remove(entity);
                    if (kvp.Value.Count == 0)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
        }

        public void UpdatePosition(IEntity entity, MapCoordinate mapCoordinate, MapCoordinate oldMapCoordinate)
        {
            this[oldMapCoordinate].Remove(entity);
            Add(mapCoordinate, entity);

            if (this[oldMapCoordinate].Count == 0)
            {
                Remove(oldMapCoordinate);
            }
        }
    }
}