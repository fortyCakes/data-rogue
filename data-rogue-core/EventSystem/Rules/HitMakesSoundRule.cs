using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class HitMakesSoundRule : IEventRule
    {
        private ISoundSystem _soundSystem;
        private IPrototypeSystem _prototypeSystem;

        public HitMakesSoundRule(ISystemContainer systemContainer)
        {
            _soundSystem = systemContainer.SoundSystem;
            _prototypeSystem = systemContainer.PrototypeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.AttackClass == "Heavy" || data.AttackClass == "Light")
            {
                var sound = _prototypeSystem.Get("Sound:Strike");
                _soundSystem.PlaySound(sound);
            }

            return true;
        }
    }
}