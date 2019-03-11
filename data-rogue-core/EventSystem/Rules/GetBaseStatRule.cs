using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class GetBaseStatRule : IEventRule
    {
        private readonly IPlayerSystem _playerSystem;

        public GetBaseStatRule(ISystemContainer systemContainer)
        {
            _playerSystem = systemContainer.PlayerSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public int RuleOrder => int.MaxValue;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case Stat.Muscle:
                    data.Value = sender.Get<Fighter>().Muscle;
                    break;
                case Stat.Agility:
                    data.Value = sender.Get<Fighter>().Agility;
                    break;
                case Stat.Willpower:
                    data.Value = sender.Get<Fighter>().Willpower;
                    break;
                case Stat.Intellect:
                    data.Value = sender.Get<Fighter>().Intellect;
                    break;
                case Stat.Tension:
                    if (sender != _playerSystem.Player)
                    {
                        throw new ApplicationException("Only the Player can check tension.");
                    }
                    data.Value = 0;
                    break;
                default:
                    throw new ApplicationException($"Could not resolve stat {data.Stat.ToString()}");
            }

            return true;
        }
    }
}
