using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using data_rogue_core.Data;
using data_rogue_core.Behaviours;

namespace data_rogue_core.Systems
{
    public class PrototypeSystem : BaseSystem, IPrototypeSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Prototype) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { };

        public PrototypeSystem(IEntityEngine engine, IPositionSystem positionSystem, IBehaviourFactory behaviourFactory)
        {
            Engine = engine;
            PositionSystem = positionSystem;
            BehaviourFactory = behaviourFactory;
        }

        public IEntityEngine Engine;
        public IPositionSystem PositionSystem;
        private IBehaviourFactory BehaviourFactory;

        public IEntity Get(int entityId)
        {
            var entity = Entities.Single(e => e.EntityId == entityId);

            return MakeInstanceOf(entity);
        }

        public IEntity Get(string entityName)
        {
            var entity = Entities.Single(e => e.Get<Prototype>().Name == entityName);

            return MakeInstanceOf(entity);
        }

        public IEntity Get(IEntity entity)
        {
            return MakeInstanceOf(entity);
        }

        public IEntity CreateAt(string entityName, MapCoordinate mapCoordinate)
        {
            var entity = Get(entityName);
            PositionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        public IEntity CreateAt(IEntity entity, MapCoordinate mapCoordinate)
        {
            var newEntity = Get(entity);
            PositionSystem.SetPosition(newEntity, mapCoordinate);
            return newEntity;
        }

        public IEntity CreateAt(int entityId, MapCoordinate mapCoordinate)
        {
            var entity = Get(entityId);
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

                IEntityComponent newComponent;

                if (IsBehaviour(componentType))
                {
                    newComponent = BehaviourFactory.Get(componentType);
                }
                else {
                    newComponent = (IEntityComponent)Activator.CreateInstance(componentType);
                }

                foreach (FieldInfo fieldInfo in componentType.GetFields())
                {
                    if (fieldInfo.FieldType == typeof(Counter))
                    {
                        Counter oldValue = (Counter)fieldInfo.GetValue(component);
                        fieldInfo.SetValue(newComponent, new Counter { Current = oldValue.Current, Max = oldValue.Max });
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

        private static bool IsBehaviour(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IBehaviour));
        }
    }
}
