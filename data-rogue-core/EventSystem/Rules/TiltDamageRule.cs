﻿using System;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class TiltDamageRule : IEventRule
    {
        public TiltDamageRule(IEventRuleSystem eventRuleSystem, IMessageSystem messageSystem)
        {
            EventRuleSystem = eventRuleSystem;
            MessageSystem = messageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Damage };
        public int RuleOrder => 1;

        private IEventRuleSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var fighter = sender.Get<Fighter>();
            var data = eventData as DamageEventData;

            var tiltMissing = fighter.Tilt.Max - fighter.Tilt.Current;

            if (tiltMissing <= 0)
                return true;

            var ratio = 1;

            var tiltDamage = data.Damage / ratio;

            var newTilt = fighter.Tilt.Current + tiltDamage;

            var causedBreak = false;

            if (newTilt > fighter.Tilt.Max)
            {
                causedBreak = true;

                var tiltOver = newTilt - fighter.Tilt.Max;

                data.Damage = (int)Math.Floor((decimal)tiltOver * ratio);

                fighter.BreakCounter = 5;
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

            return false;
        }
    }
}
