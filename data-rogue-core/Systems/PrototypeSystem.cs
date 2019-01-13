using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Data;

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

        public IEntity Create(int entityId)
        {
            var entity = this.Entities.Single(e => e.EntityId == entityId);

            return MakeInstanceOf(entity);
        }

        public IEntity Create(string entityName)
        {
            var entity = this.Entities.Single(e => e.Name == entityName);

            return MakeInstanceOf(entity);
        }

        public IEntity Create(IEntity entity)
        {
            return MakeInstanceOf(entity);
        }

        public IEntity CreateAt(string entityName, MapCoordinate mapCoordinate)
        {
            var entity = Create(entityName);
            PositionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        public IEntity CreateAt(IEntity entity, MapCoordinate mapCoordinate)
        {
            var newEntity = Create(entity);
            PositionSystem.SetPosition(newEntity, mapCoordinate);
            return newEntity;
        }

        public IEntity CreateAt(int entityId, MapCoordinate mapCoordinate)
        {
            var entity = Create(entityId);
            PositionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        private IEntity MakeInstanceOf(IEntity entity)
        {
            if (entity.Get<Prototype>().Singleton)
            {
                return entity;
            }

            var newComponents = new List<IEntityComponent>();

            foreach(var component in entity.Components.Where(c => c.GetType() != typeof(Prototype)))
            {
                var componentType = component.GetType();

                IEntityComponent newComponent = (IEntityComponent)Activator.CreateInstance(componentType);

                foreach (FieldInfo fieldInfo in componentType.GetFields())
                {
                    if (fieldInfo.FieldType == typeof(StatCounter))
                    {
                        StatCounter oldValue = (StatCounter)fieldInfo.GetValue(component);
                        fieldInfo.SetValue(newComponent, new StatCounter{Current = oldValue.Current, Max = oldValue.Max} );
                    }
                    else
                    {
                        var oldValue = fieldInfo.GetValue(component);
                        fieldInfo.SetValue(newComponent, oldValue);
                    }
                }

                newComponents.Add(newComponent);
            }

            return Engine.New(null, newComponents.ToArray());
        }

    }
}
