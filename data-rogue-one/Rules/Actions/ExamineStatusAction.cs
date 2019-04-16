using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
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

            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, GetStatusConfigurations(entity), entity, true));

            return false;
        }

        private List<StatsConfiguration> GetStatusConfigurations(IEntity entity)
        {
            var statsDisplays = new List<StatsDisplay>
            {
                new StatsDisplay { DisplayType = "AppearanceName" },
                new StatsDisplay { DisplayType = "Spacer" }
            };

            if (entity.Has<Description>())
            {
                statsDisplays.Add(new StatsDisplay {DisplayType =  "Text", Parameters = entity.Get<Description>().Detail });
            }

            if (entity.Has<Experience>())
            {
                statsDisplays.Add(new StatsDisplay {DisplayType = "Experience"});
            }

            if (entity.Has<Health>())
            {
                statsDisplays.AddRange(GetCombatStats());
            }

            return new List<StatsConfiguration>
            {
                new StatsConfiguration
                {
                    Position = new Rectangle(4,4,0,0),
                    Displays = statsDisplays
                }
            };
        }

        private static List<StatsDisplay> GetCombatStats()
        {
            var combatStats = new List<StatsDisplay>
            {
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "Health,HP", BackColor = Color.DarkRed},
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                new StatsDisplay { DisplayType =  "Stat", Parameters = "Tension" },
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new StatsDisplay {DisplayType = "Spacer"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "Muscle"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "Agility"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "Intellect"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "Willpower"},
                new StatsDisplay {DisplayType = "Spacer"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "AC"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "EV"},
                new StatsDisplay {DisplayType = "Stat", Parameters = "SH"}
            };
            return combatStats;
        }
    }
}