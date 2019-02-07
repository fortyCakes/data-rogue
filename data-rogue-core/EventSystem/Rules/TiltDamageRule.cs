using System;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class TiltDamageRule : IEventRule
    {
        public TiltDamageRule(ISystemContainer systemContainer)
        {
            EventRuleSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Damage };
        public int RuleOrder => 1;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var fighter = sender.Get<Fighter>();
            var data = eventData as DamageEventData;

            var tiltMissing = fighter.Tilt.Max - fighter.Tilt.Current;

            if (fighter.BrokenTicks > 0)
                return true;

            var ratio = 1;

            var tiltDamage = data.Damage / ratio;

            var newTilt = fighter.Tilt.Current + tiltDamage;

            var causedBreak = false;

            if (newTilt > fighter.Tilt.Max)
            {
                tiltDamage = fighter.Tilt.Max - fighter.Tilt.Current;

                causedBreak = true;

                var tiltOver = newTilt - fighter.Tilt.Max;

                data.Damage = (int)Math.Floor((decimal)tiltOver * ratio);

                fighter.BrokenTicks = 5000 - 1;
            }
            else
            {
                data.Damage = 0;
            }

            fighter.Tilt.Current = Math.Min(fighter.Tilt.Max, newTilt);

            var msg = $"{sender.Get<Description>().Name} takes {tiltDamage} tilt.";

            if (causedBreak)
            {
                msg += " Their defences are broken!";
            }

            MessageSystem.Write(msg, Color.DarkCyan);

            if (data.Overwhelming && data.Damage > 0)
                return true;

            return false;
        }
    }
}
