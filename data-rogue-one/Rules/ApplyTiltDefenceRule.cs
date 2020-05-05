using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public abstract class ApplyTiltDefenceRule: IEventRule
    {
        private ISystemContainer _systemContainer;
        private readonly string _defenceName;
        private readonly string _statName;

        public ApplyTiltDefenceRule(ISystemContainer systemContainer, string statName, string defenceName)
        {
            _systemContainer = systemContainer;
            _defenceName = defenceName;
            _statName = statName;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Defence };
        public abstract List<string> ValidAttackClasses { get; }

        protected abstract decimal ApplyTiltToDefence(TiltFighter tiltFighter, decimal defence);
        protected abstract void IncreaseTiltForSucessfulDefence(TiltFighter tiltFighter, decimal defenceRoll, DefenceEventData eventData);

        public uint RuleOrder => 488;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as DefenceEventData;

            if (data.DefenceType != _defenceName)
            {
                // Don't try to apply it
                return true;
            }

            var attackClass = data.ForAttack.AttackClass;
            if (!ValidAttackClasses.Contains(attackClass))
            {
                return true;
            }

            var defender = data.ForAttack.Defender;
            TiltFighter tiltFighter = defender.Get<TiltFighter>();
            var isBroken = tiltFighter.BrokenTicks > 0;

            if (isBroken)
            {
                return true;
            }

            var defence = _systemContainer.EventSystem.GetStat(defender, _statName);
            defence = ApplyTiltToDefence(tiltFighter, defence);

            var defenceRoll = _systemContainer.Random.Between(1,20);

            if (defence + defenceRoll > data.ForAttack.AttackRoll)
            {
                IncreaseTiltForSucessfulDefence(tiltFighter, defenceRoll, data);
                return false;
            }

            return true;
        }

    }

    public class ApplyTiltTankDefenceRule : ApplyTiltDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Blast", "Launcher", "Thrown" };
        protected override decimal ApplyTiltToDefence(TiltFighter tiltFighter, decimal defence)
        {
            var tiltPercent = (decimal)tiltFighter.Tilt.Current / tiltFighter.Tilt.Max;

            return defence * (1m - 0.5m * tiltPercent);
        }

        protected override void IncreaseTiltForSucessfulDefence(TiltFighter tiltFighter, decimal defenceRoll, DefenceEventData eventData)
        {
            tiltFighter.Tilt.Add((int)defenceRoll);
        }

        public ApplyTiltTankDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "AC", "Tank")
        {
        }
    }

    public class ApplyTiltDodgeDefenceRule : ApplyTiltDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Bolt", "Launcher", "Thrown" };

        public ApplyTiltDodgeDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "EV", "Dodge")
        {
        }
        protected override decimal ApplyTiltToDefence(TiltFighter tiltFighter, decimal defence)
        {
            var tiltPercent = (decimal)tiltFighter.Tilt.Current / tiltFighter.Tilt.Max;

            return defence * (1m - 0.2m * tiltPercent);
        }

        protected override void IncreaseTiltForSucessfulDefence(TiltFighter tiltFighter, decimal defenceRoll,  DefenceEventData eventData)
        {
            tiltFighter.Tilt.Add(20);
        }
    }

    public class ApplyTiltBlockDefenceRule : ApplyTiltDefenceRule
    {
        public override List<string> ValidAttackClasses => new List<string> { "Heavy", "Light", "Bolt", "Blast", "Launcher", "Thrown" };

        public ApplyTiltBlockDefenceRule(ISystemContainer systemContainer) : base(systemContainer, "SH", "Block")
        {
        }

        protected override decimal ApplyTiltToDefence(TiltFighter tiltFighter, decimal defence)
        {
            var tiltPercent = (decimal)tiltFighter.Tilt.Current / tiltFighter.Tilt.Max;

            return defence * (1.25m - 0.5m * tiltPercent);
        }

        protected override void IncreaseTiltForSucessfulDefence(TiltFighter tiltFighter, decimal defenceRoll, DefenceEventData eventData)
        {
            var tiltAdded = 15;

            tiltAdded += (int)eventData.ForAttack.Damage / 10;

            tiltFighter.Tilt.Add(tiltAdded);
        }
    }
}
