using System;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    public class GetBaseStatRule : IEventRule
    {
        private readonly IPlayerSystem _playerSystem;
        private readonly IStatSystem _statSystem;

        public GetBaseStatRule(ISystemContainer systemContainer)
        {
            _playerSystem = systemContainer.PlayerSystem;
            _statSystem = systemContainer.StatSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public int RuleOrder => int.MaxValue;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case "Tension":
                    if (sender != _playerSystem.Player)
                    {
                        throw new ApplicationException("Only the Player can check tension.");
                    }
                    data.Value = 0;
                    break;
                default:
                    data.Value = _statSystem.GetEntityStat(sender, data.Stat);
                    break;
            }

            return true;
        }
    }

    public class AddAgilityToEvasionRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public AddAgilityToEvasionRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GetStat };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            if (data.Stat == "EV")
            {
                data.Value += _systemContainer.EventSystem.GetStat(sender, "Agility");
            }

            return true;
        }
    }
}
