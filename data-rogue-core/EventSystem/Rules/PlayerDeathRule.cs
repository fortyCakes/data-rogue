﻿using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class PlayerDeathRule : IEventRule
    {
        public PlayerDeathRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Death };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;

        private readonly ISystemContainer systemContainer;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (systemContainer.PlayerSystem.IsPlayer(sender))
            {
                systemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(
                        systemContainer.ActivitySystem,
                        systemContainer.PlayerSystem,
                        systemContainer.SaveSystem
                    )));
                systemContainer.ActivitySystem.Push(new EndGameScreenActivity(systemContainer, false));
            }

            return true;
        }
    }
}
