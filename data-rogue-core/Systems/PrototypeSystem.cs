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

        public PrototypeSystem(IEntityEngine engine, IPositionSystem positionSystem, ISystemContainer systemContainer)
        {
            _engine = engine;
            _positionSystem = positionSystem;
            _systemContainer = systemContainer;
        }

        private IEntityEngine _engine;
        private IPositionSystem _positionSystem;
        private ISystemContainer _systemContainer;

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

        public IEntity GetPrototype(string entityName)
        {
            return Entities.Single(e => e.Get<Prototype>().Name == entityName);
        }

        public IEntity TryGet(string entityName)
        {
            var entity = Entities.SingleOrDefault(e => e.Get<Prototype>().Name == entityName);

            if (entity == null) return null;

            return MakeInstanceOf(entity);
        }

        public IEntity Get(IEntity entity)
        {
            return MakeInstanceOf(entity);
        }

        public IEntity CreateAt(string entityName, MapCoordinate mapCoordinate)
        {
            var entity = Get(entityName);
            _positionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        public IEntity CreateAt(IEntity entity, MapCoordinate mapCoordinate)
        {
            var newEntity = Get(entity);
            _positionSystem.SetPosition(newEntity, mapCoordinate);
            return newEntity;
        }

        public IEntity CreateAt(int entityId, MapCoordinate mapCoordinate)
        {
            var entity = Get(entityId);
            _positionSystem.SetPosition(entity, mapCoordinate);
            return entity;
        }

        private IEntity MakeInstanceOf(IEntity entity)
        {
            if (entity.Get<Prototype>().Singleton)
            {
                return entity;
            }

            var newComponents = new List<IEntityComponent>();
            var startWithItems = new List<StartsWithItem>();

            foreach(var component in entity.Components.Where(c => c.GetType() != typeof(Prototype)))
            {
                if (component is StartsWithItem)
                {
                    startWithItems.Add((StartsWithItem)component);
                }
                else
                {
                    var componentType = component.GetType();

                    IEntityComponent newComponent;

                    if (IsBehaviour(componentType))
                    {
                        newComponent = (IEntityComponent)Activator.CreateInstance(componentType, new object[] { _systemContainer });
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
            }

            var newEntity = _engine.New(null, newComponents.ToArray());

            var inventory = newEntity.Get<Inventory>();

            foreach (var startItem in startWithItems)
            {
                var item = Get(startItem.Item);
                
                _systemContainer.ItemSystem.AddItemDirectlyToInventory(item, inventory);

                if (startItem.Equipped)
                {
                    _systemContainer.EquipmentSystem.Equip(newEntity, item);
                }
            }

            return newEntity;
        }

        private static bool IsBehaviour(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IBehaviour));
        }
    }
}
