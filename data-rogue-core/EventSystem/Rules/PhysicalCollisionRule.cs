﻿using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class PhysicalCollisionRule : IEventRule
    {
        public PhysicalCollisionRule(ISystemContainer systemContainer)
        {
            PositionSystem = systemContainer.PositionSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public uint RuleOrder => 0;

        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        private IPositionSystem PositionSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (IsSolid(sender))
            {
                var vector = (Vector) eventData;
                var targetCoordinate = PositionSystem.CoordinateOf(sender) + vector;

                var entitiesAtPosition = PositionSystem.EntitiesAt(targetCoordinate);

                if (entitiesAtPosition.Any(IsSolid))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsSolid(IEntity e)
        {
            return e.Get<Physical>()?.Passable == false;
        }
    }
}
