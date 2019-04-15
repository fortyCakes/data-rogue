using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.IOSystems;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_one.EventSystem.Rules
{
    public class PlayerStatusAction: ApplyActionRule
    {
        public PlayerStatusAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.PlayerStatus;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, GetStatusConfigurations(), true));

            return false;
        }

        private List<StatsConfiguration> GetStatusConfigurations()
        {
            return new List<StatsConfiguration>
            {
                new StatsConfiguration
                {
                    Position = new Rectangle(2,2,0,0),
                    Displays = new List<StatsDisplay>
                    {
                        new StatsDisplay { DisplayType = "LargeText", Parameters = "Character Status" },
                        new StatsDisplay { DisplayType = "Spacer" },
                        new StatsDisplay { DisplayType = "Name" },
                        new StatsDisplay { DisplayType = "Stat", Parameters = "Muscle"},
                        new StatsDisplay { DisplayType = "Stat", Parameters = "Agility"},
                        new StatsDisplay { DisplayType = "Stat", Parameters = "Intellect"},
                        new StatsDisplay { DisplayType = "Stat", Parameters = "Willpower"},
                        new StatsDisplay { DisplayType = "Spacer" },
                        new StatsDisplay { DisplayType = "Stat", Parameters = "AC"},
                        new StatsDisplay { DisplayType = "Stat", Parameters = "EV"},
                        new StatsDisplay { DisplayType = "Stat", Parameters = "SH"},

                    }
                }
            };
        }
    }
}