using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class PlayerVictoryRule : IEventRule
    {
        public PlayerVictoryRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Victory };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;

        private readonly ISystemContainer systemContainer;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (systemContainer.PlayerSystem.IsPlayer(sender))
            {
                systemContainer.ActivitySystem.GameplayActivity.Running = false;

                systemContainer.ActivitySystem.Push(new MenuActivity(
                    systemContainer.ActivitySystem.DefaultPosition,
                    systemContainer.ActivitySystem.DefaultPadding,
                    new MainMenu(systemContainer)));
                systemContainer.ActivitySystem.Push(new EndGameScreenActivity(
                    systemContainer.ActivitySystem.DefaultPosition,
                    systemContainer.ActivitySystem.DefaultPadding,
                    systemContainer, true));
            }

            return true;
        }
    }
}
