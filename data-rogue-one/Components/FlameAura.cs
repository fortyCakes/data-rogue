using System.Linq;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Components
{
    public class FlameAura : IEntityComponent, ITickUpdate
    {
        public void Tick(ISystemContainer systemContainer, IEntity entity, ulong currentTime)
        {
            if (IsFlameTick(currentTime))
            {
                var location = systemContainer.PositionSystem.CoordinateOf(entity);
                var locations = location.AdjacentCells();
                locations.Add(location);
                var adjacentEntities = locations.SelectMany(l => systemContainer.PositionSystem.EntitiesAt(l));

                foreach (var defender in adjacentEntities)
                {
                    if (defender.Has<Health>() && !defender.Has<FlameAura>())
                    {
                        var attackData = new AttackEventData
                        {
                            Accuracy = 100,
                            AttackClass = "Blast",
                            Attacker = entity,
                            AttackName = "Flame Aura",
                            Damage = 2,
                            Defender = defender,
                            Speed = 0,
                            SpendTime = false,
                            Tags = new[] {"Fire"}
                        };

                        systemContainer.EventSystem.Try(EventType.Attack, entity, attackData);
                    }
                }
            }
        }

        private static bool IsFlameTick(ulong currentTime)
        {
            return currentTime % 1000 == 0;
        }
    }
}
