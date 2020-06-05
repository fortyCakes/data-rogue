using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class PlayerStepMakesSoundRule : IEventRule
    {
        private ISoundSystem _soundSystem;
        private IPrototypeSystem _prototypeSystem;

        public PlayerStepMakesSoundRule(ISystemContainer systemContainer)
        {
            _soundSystem = systemContainer.SoundSystem;
            _prototypeSystem = systemContainer.PrototypeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (sender.IsPlayer)
            {
                var sound = _prototypeSystem.Get("Sound:Step");
                _soundSystem.PlaySound(sound);
            }

            return true;
        }
    }
}
