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

        public FighterSystem(IEntityEngine engine, IMessageSystem messageSystem, IEventRuleSystem eventRuleSystem, ITimeSystem timeSystem)
        {
            Engine = engine;
            MessageSystem = messageSystem;
            EventRuleSystem = eventRuleSystem;
            TimeSystem = timeSystem;
        }

        public IEntityEngine Engine { get; }
        public IMessageSystem MessageSystem { get; }
        public IEventRuleSystem EventRuleSystem { get; }
        public ITimeSystem TimeSystem { get; }

        public void BasicAttack(IEntity attacker, IEntity defender)
        {
            var attackingFighter = attacker.Get<Fighter>();
            var defendingFighter = defender.Get<Fighter>();

            var attackerDescription = attacker.Get<Description>();
            var defenderDescription = defender.Get<Description>();

            var hit = EventRuleSystem.Try(EventType.Attack, attacker, defender);

            if (hit)
            {
                var baseDamage = attackingFighter.Muscle;
                EventRuleSystem.Try(EventType.Damage, defender, new DamageEventData{Damage = baseDamage});
            }

            EventRuleSystem.Try(EventType.SpendTime, attacker, 1000);
        }
    }
}
