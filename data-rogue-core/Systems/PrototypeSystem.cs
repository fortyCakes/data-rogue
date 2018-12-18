using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems
{
    public class PrototypeSystem : BaseSystem, IPrototypeSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Prototype) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { };

        public PrototypeSystem(IEntityEngine engine, IPositionSystem positionSystem)
        {
            Engine = engine;
            PositionSystem = positionSystem;
        }

        public IEntityEngine Engine { get; }
        public IPositionSystem PositionSystem { get; }

        public IEntity Create(int entityId, string newName)
        {
            var entity = this.Entities.Single(e => e.EntityId == entityId);

            return MakeInstanceOf(entity, newName);
        }

        public IEntity Create(string entityName, string newName)
        {
            var entity = this.Entities.Single(e => e.Name == entityName);

            return MakeInstanceOf(entity, newName);
        }

        public IEntity Create(IEntity entity, string newName)
        {
            return MakeInstanceOf(entity, newName);
        }

        public IEntity CreateAt(string entityName, string newName, MapCoordinate mapCoordinate)
        {
            var entity = Create(entityName, newName);
            PositionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        public IEntity CreateAt(IEntity entity, string newName, MapCoordinate mapCoordinate)
        {
            var newEntity = Create(entity, newName);
            PositionSystem.SetPosition(newEntity, mapCoordinate);
            return newEntity;
        }

        public IEntity CreateAt(int entityId, string newName, MapCoordinate mapCoordinate)
        {
            var entity = Create(entityId, newName);
            PositionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        private IEntity MakeInstanceOf(IEntity entity, string withName)
        {
            var newComponents = new List<IEntityComponent>();

            foreach(var component in entity.Components.Where(c => c.GetType() != typeof(Prototype)))
            {
                var componentType = component.GetType();

                IEntityComponent newComponent = (IEntityComponent)Activator.CreateInstance(componentType);

                foreach (FieldInfo fieldInfo in componentType.GetFields())
                {
                    var oldValue = fieldInfo.GetValue(component);
                    fieldInfo.SetValue(newComponent, oldValue);
                }

                newComponents.Add(newComponent);
            }

            return Engine.New(withName, newComponents.ToArray());
        }

    }
}
