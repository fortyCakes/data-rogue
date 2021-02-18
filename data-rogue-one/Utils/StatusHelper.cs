using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;

namespace data_rogue_one.EventSystem.Utils
{
    public static class StatusHelper
    {
        public static List<StatsConfiguration> GetStatusConfigurations(IEntity entity)
        {
            var statsDisplays = new List<InfoDisplay>
            {
                new InfoDisplay { ControlType = typeof(AppearanceName) },
                new InfoDisplay { ControlType = typeof(Spacer) }
            };

            if (entity.Has<Description>())
            {
                statsDisplays.Add(new InfoDisplay {ControlType = typeof(TextControl), Parameters = entity.Get<Description>().Detail });


                if (entity.Has<Item>())
                {

                    statsDisplays.Add(new InfoDisplay { ControlType = typeof(Spacer) });
                    statsDisplays.Add(new InfoDisplay { ControlType = typeof(TextControl), Parameters = ItemStatsDescriber.Describe(entity) });
                }

                statsDisplays.Add(new InfoDisplay { ControlType = typeof(Spacer) });
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

        public static List<InfoDisplay> GetCombatStats(IEntity entity)
        {
            var ret = new List<InfoDisplay>();

            var healthStats = new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.DarkRed },
                new InfoDisplay { ControlType = typeof(Spacer) }};

            var auraStats = new List<InfoDisplay>
            {
                new InfoDisplay {ControlType = typeof(ComponentCounter), Parameters = "AuraFighter,Aura", BackColor = Color.Yellow}
            };

            if (entity.IsPlayer)
            {
                auraStats.Add(new InfoDisplay { ControlType = typeof(StatControl), Parameters = "Tension" });
            }
            auraStats.Add(new InfoDisplay { ControlType = typeof(Spacer)});

            var aegisStats = new List<InfoDisplay>()
            {
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "CurrentAegisLevel"},
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "Aegis"},
                new InfoDisplay {ControlType = typeof(Spacer)},
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
                new InfoDisplay {ControlType = typeof(StatControl), Parameters = "SH"},
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

            if (entity.Has<AegisRecovery>())
            {
                ret.AddRange(aegisStats);
            }

            ret.AddRange(combatStats);

            return ret;
        }
    }
}