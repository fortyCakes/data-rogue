using System;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class RandomiseDamageRule: IEventRule
    {
        private const int RANDOM_ROLLS = 3;
        private const int RANDOMISATION_DENOMINATOR = 8;

        public RandomiseDamageRule(ISystemContainer systemContainer)
        {
            _random = systemContainer.Random;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Damage };
        public int RuleOrder => 100;

        private IRandom _random;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as DamageEventData;
            int randomiseBy = Math.Max(data.Damage / RANDOMISATION_DENOMINATOR, 1);

            for (int i = 0; i < RANDOM_ROLLS; i++)
            {
                data.Damage += _random.Between(-randomiseBy, +randomiseBy);
            }

            return true;
        }
    }
}
