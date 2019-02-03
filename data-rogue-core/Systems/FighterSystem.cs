using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Systems
{
    public class FighterSystem : BaseSystem, IFighterSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Fighter) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public FighterSystem(IEntityEngine engine, IMessageSystem messageSystem, IEventSystem eventRuleSystem, ITimeSystem timeSystem)
        {
            Engine = engine;
            MessageSystem = messageSystem;
            EventRuleSystem = eventRuleSystem;
            TimeSystem = timeSystem;
        }

        public IEntityEngine Engine { get; }
        public IMessageSystem MessageSystem { get; }
        public IEventSystem EventRuleSystem { get; }
        public ITimeSystem TimeSystem { get; }

        public void BasicAttack(IEntity attacker, IEntity defender)
        {
            var attackingFighter = attacker.Get<Fighter>();
            var defendingFighter = defender.Get<Fighter>();

            var attackData = new AttackEventData { Defender = defender };

            var hit = EventRuleSystem.Try(EventType.Attack, attacker, attackData);

            var msg = $"{attacker.Get<Description>().Name} attacks {defender.Get<Description>().Name}";

            if (hit)
            {
                var baseDamage = attackingFighter.Muscle;
                msg += $" and hits for {baseDamage} damage.";

                EventRuleSystem.Try(EventType.Damage, defender, new DamageEventData{Damage = baseDamage});
            }
            else
            {
                msg += $" and misses.";
            }

            MessageSystem.Write(msg);

            EventRuleSystem.Try(EventType.SpendTime, attacker, new SpendTimeEventData() {Ticks = 1000});
        }

        public IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities)
        {
            return entities.Where(e => Entities.Contains(e));
        }
    }
}
