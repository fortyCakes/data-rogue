using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public abstract class ApplyDefenceRule: IEventRule
    {
        private ISystemContainer _systemContainer;
        private readonly string _defenceName;
        private readonly string _statName;

        public ApplyDefenceRule(ISystemContainer systemContainer, string statName, string defenceName)
        {
            _systemContainer = systemContainer;
            _defenceName = defenceName;
            _statName = statName;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Defence };
        public abstract List<string> ValidAttackClasses { get; }
        public int RuleOrder => 488;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as DefenceEventData;

            if (data.DefenceType != _defenceName)
            {
                // Don't try to apply it
                return false;
            }

            var attackClass = data.ForAttack.AttackClass;
            if (!ValidAttackClasses.Contains(attackClass))
            {
                return false;
            }

            var defender = data.ForAttack.Defender;
            var isBroken = defender.Get<TiltFighter>().BrokenTicks > 0;

            if (isBroken)
            {
                return false;
            }

            var defence = _systemContainer.EventSystem.GetStat(defender, _statName);

            var defenceRoll = _systemContainer.Random.StatCheck(defence);

            if (defenceRoll > data.ForAttack.AttackRoll || defenceRoll == defence + 20)
            {
                return true;
            }

            return false;
        }
    }

    public class ApplyTankDefenceRule : ApplyDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Blast", "Launcher", "Thrown" };

        public ApplyTankDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "AC", "Tank")
        {
        }
    }

    public class ApplyDodgeDefenceRule : ApplyDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Bolt", "Launcher", "Thrown" };

        public ApplyDodgeDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "EV", "Dodge")
        {
        }
    }

    public class ApplyBlockDefenceRule : ApplyDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Bolt", "Blast", "Launcher", "Thrown" };

        public ApplyBlockDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "SH", "Block")
        {
        }
    }
}
