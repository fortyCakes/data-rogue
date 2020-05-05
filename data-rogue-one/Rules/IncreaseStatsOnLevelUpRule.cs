using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_one.Rules
{
    public class IncreaseStatsOnLevelUpRule: IEventRule
    {
        private ISystemContainer systemContainer;

        public IncreaseStatsOnLevelUpRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }
        public EventTypeList EventTypes => new EventTypeList { EventType.LevelUp };

        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            systemContainer.StatSystem.IncreaseStat(sender, "Muscle", 2);
            systemContainer.StatSystem.IncreaseStat(sender, "Agility", 2);
            systemContainer.StatSystem.IncreaseStat(sender, "Willpower", 2);
            systemContainer.StatSystem.IncreaseStat(sender, "Intellect", 2);

            var tilt = sender.Get<TiltFighter>();
            tilt.BrokenTicks = 0;
            tilt.Tilt.Current = 0;
            tilt.Tilt.Max += 10;

            var aura = sender.Get<AuraFighter>();
            aura.BaseAura += 10;
            aura.Aura.Max += 100;
            aura.Aura.Current = aura.Aura.Max;

            var health = sender.Get<Health>();
            health.HP.Max += 10;
            health.HP.Current = health.HP.Max;

            systemContainer.MessageSystem.Write("Stats increased by 2! Health, Aura and Tilt reserves have increased and been refreshed.");

            return true;
        }
    }
}