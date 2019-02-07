using System.Collections.Generic;
using System.Linq;
using System.Text;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

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

        public IEntity Build(ISystemContainer systemContainer)
        {
            var components = Components.Select(c => ComponentSerializer.Deserialize(systemContainer, c.ToString(), 0)).ToArray();

            if (Id.HasValue)
            {
                return systemContainer.EntityEngine.Load(Id.Value, new Entity(Id.Value, Name, components));
            }
            else
            {
                return systemContainer.EntityEngine.New(Name, components);
            }
        }
    }
}
