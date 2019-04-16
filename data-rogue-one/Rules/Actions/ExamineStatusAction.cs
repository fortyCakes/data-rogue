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
    public class ExamineStatusAction: ApplyActionRule
    {
        public ExamineStatusAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Examine;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var entityId = uint.Parse(eventData.Parameters);
            var entity = _systemContainer.EntityEngine.Get(entityId);

            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, GetStatusConfigurations(), entity, true));

            return false;
        }

        private List<StatsConfiguration> GetStatusConfigurations()
        {
            return new List<StatsConfiguration>
            {
                new StatsConfiguration
                {
                    Position = new Rectangle(2,4,0,0),
                    Displays = new List<StatsDisplay>
                    {
                        new StatsDisplay { DisplayType = "AppearanceName" },
                        new StatsDisplay { DisplayType = "Spacer" },
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