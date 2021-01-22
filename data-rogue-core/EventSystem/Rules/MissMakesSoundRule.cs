using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class MissMakesSoundRule : IEventRule
    {
        private ISoundSystem _soundSystem;
        private IPrototypeSystem _prototypeSystem;

        public MissMakesSoundRule(ISystemContainer systemContainer)
        {
            _soundSystem = systemContainer.SoundSystem;
            _prototypeSystem = systemContainer.PrototypeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.AfterFailure;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.AttackClass == "Heavy" || data.AttackClass == "Light")
            {
                var sound = _prototypeSystem.Get("Sound:Miss");
                _soundSystem.PlaySound(sound);
            }

            return true;
        }
    }
}