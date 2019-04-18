using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
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
            var statsDisplays = new List<InfoDisplay>
            {
                new InfoDisplay { ControlType = typeof(AppearanceName) },
                new InfoDisplay { ControlType = typeof(Spacer) }
            };

            if (entity.Has<Description>())
            {
                statsDisplays.Add(new InfoDisplay {ControlType = typeof(TextControl), Parameters = entity.Get<Description>().Detail });
            }

            if (entity.Has<Experience>())
            {
                statsDisplays.Add(new InfoDisplay {ControlType = typeof(ExperienceControl)});
            }

            if (entity.Has<Health>())
            {
                statsDisplays.AddRange(GetCombatStats(entity));
            }

            return new List<StatsConfiguration>
            {
                new StatsConfiguration
                {
                    Position = new Rectangle(0,0,0,0),
                    Displays = statsDisplays
                }
            };
        }

        private static List<InfoDisplay> GetCombatStats(IEntity entity)
        {
            var ret = new List<InfoDisplay>();

            var healthStats = new List<InfoDisplay> {
            new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.DarkRed },
                new InfoDisplay { ControlType = typeof(Spacer) }};

            var auraStats = new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "AuraFighter,Aura", BackColor = Color.Yellow },
                new InfoDisplay { ControlType = typeof(StatControl), Parameters = "Tension" },
                new InfoDisplay { ControlType = typeof(Spacer) }
            };

            var tiltStats =  new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "TiltFighter,Tilt", BackColor = Color.Purple },
                new InfoDisplay { ControlType = typeof(Spacer) } };

            var combatStats = new List<InfoDisplay>
            {
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "Muscle"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "Agility"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "Intellect"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "Willpower"},
                new InfoDisplay {ControlType = typeof(Spacer)},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "AC"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "EV"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "SH"}
            };

            if (entity.Has<Health>())
            {
                ret.AddRange(healthStats);
            }

            if (entity.Has<AuraFighter>())
            {
                ret.AddRange(auraStats);
            }

            if (entity.Has<TiltFighter>())
            {
                ret.AddRange(tiltStats);
            }

            ret.AddRange(combatStats);

            return ret;
        }
    }
}