using System.Collections.Generic;
using System.Linq;
using System.Text;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Data
{
    public class EntityBuilder
    {
        public string Name;
        private uint? Id;

        public List<StringBuilder> Components = new List<StringBuilder>();

        public EntityBuilder(string entityName)
        {
            this.Name = entityName;
        }

        public EntityBuilder(string entityName, uint entityId)
        {
            this.Name = entityName;
            this.Id = entityId;
        }

        public Entity Build(IEntityEngine engine)
        {
            var components = Components.Select(c => ComponentSerializer.Deserialize(c.ToString(), engine, 0)).ToArray();

            if (Id.HasValue)
            {
                return engine.Load(Id.Value, new Entity(Id.Value, Name, components));
            }
            else
            {
                return engine.New(Name, components);
            }
        }
    }
}
