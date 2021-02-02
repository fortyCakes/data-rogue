using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class GetBaseStatRule : IEventRule
    {
        private readonly IPlayerSystem _playerSystem;
        private readonly IStatSystem _statSystem;
        private readonly IItemSystem _itemSystem;

        public GetBaseStatRule(ISystemContainer systemContainer)
        {
            _playerSystem = systemContainer.PlayerSystem;
            _statSystem = systemContainer.StatSystem;
            _itemSystem = systemContainer.ItemSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public uint RuleOrder => uint.MaxValue;

        public EventRuleType RuleType => EventRuleType.EventResolution;

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
                case "Score":
                    data.Value = 0;
                    break;
                default:
                    data.Value = _statSystem.GetEntityStat(sender, data.Stat);
                    break;
            }

            return true;
        }
    }
}
