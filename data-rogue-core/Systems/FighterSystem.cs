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
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Health) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        private readonly IStatSystem _statSystem;

        public FighterSystem(IEntityEngine engine, IMessageSystem messageSystem, IEventSystem eventRuleSystem, ITimeSystem timeSystem, IStatSystem statSystem)
        {
            _engine = engine;
            _messageSystem = messageSystem;
            _eventRuleSystem = eventRuleSystem;
            _timeSystem = timeSystem;
            _statSystem = statSystem;
        }

        public IEntityEngine _engine;
        public IMessageSystem _messageSystem;
        public IEventSystem _eventRuleSystem;
        public ITimeSystem _timeSystem;

        public bool BasicAttack(IEntity attacker, IEntity defender)
        {
            var baseDamage = _statSystem.GetEntityStat(attacker, "Muscle");

            return Attack(attacker, defender, attackDamage: baseDamage);
        }

        public bool Attack(IEntity attacker, IEntity defender, string attackClass = null, int? attackDamage = null, string[] attackTags = null, bool spendTime = true, IEntity weapon = null)
        {
            var attack = new AttackEventData
            {
                Attacker = attacker,
                Defender = defender,
                AttackClass = attackClass,
                Damage = attackDamage,
                Tags = attackTags,
                SpendTime = spendTime,
                Weapon = weapon
            };

            return _eventRuleSystem.Try(EventType.Attack, attacker, attack);
        }

        public IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities)
        {
            return entities.Where(e => Entities.Contains(e));
        }
    }
}
