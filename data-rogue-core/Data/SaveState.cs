using data_rogue_core.EntitySystem;
using System.Collections.Generic;

namespace data_rogue_core
{
    public class SaveState
    {
        public string CurrentMapKey { get; set; }

        public List<Entity> Entities { get; set; }
        public List<SaveMap> Maps { get; set; }
    }
}