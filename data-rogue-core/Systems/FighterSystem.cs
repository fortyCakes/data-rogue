using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Systems
{
    public class FighterSystem : BaseSystem, IFighterSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(TiltFighter) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        private readonly IStatSystem _statSystem;

        public FighterSystem(IEntityEngine engine, IMessageSystem messageSystem, IEventSystem eventRuleSystem, ITimeSystem timeSystem, IStatSystem statSystem)
        {
            _engine = engine;
            _messageSystem = messageSystem;
            this._eventRuleSystem = eventRuleSystem;
            this._timeSystem = timeSystem;
            _statSystem = statSystem;
        }

        public IEntityEngine _engine;
        public IMessageSystem _messageSystem;
        public IEventSystem _eventRuleSystem;
        public ITimeSystem _timeSystem;

        public void BasicAttack(IEntity attacker, IEntity defender)
        {
            var attackingFighter = attacker.Get<TiltFighter>();
            var defendingFighter = defender.Get<TiltFighter>();

            var attackData = new AttackEventData { Defender = defender };

            var hit = _eventRuleSystem.Try(EventType.Attack, attacker, attackData);

            var msg = $"{attacker.Get<Description>().Name} attacks {defender.Get<Description>().Name}";

            if (hit)
            {
                var baseDamage = _statSystem.GetEntityStat(attacker, "Muscle");
                msg += $" and hits for {baseDamage} damage.";

                _eventRuleSystem.Try(EventType.Damage, defender, new DamageEventData{Damage = baseDamage, DamagedBy = attacker});
            }
            else
            {
                msg += $" and misses.";
            }

            _messageSystem.Write(msg);

            _eventRuleSystem.Try(EventType.SpendTime, attacker, new SpendTimeEventData() {Ticks = 1000});
        }

        public IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities)
        {
            return entities.Where(e => Entities.Contains(e));
        }
    }
}
