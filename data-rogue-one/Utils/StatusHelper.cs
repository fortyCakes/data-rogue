using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;

namespace data_rogue_one.EventSystem.Utils
{
    public static class StatusHelper
    {
        public static List<IDataRogueControl> GetStatusConfigurations(IEntity entity)
        {
            var statsDisplays = new List<IDataRogueControl>
            {
                new AppearanceName(),
                new Spacer()
            };

            if (entity.Has<Description>())
            {
                statsDisplays.Add(new TextControl{ Parameters = entity.Get<Description>().Detail });


                if (entity.Has<Item>())
                {

                    statsDisplays.Add(new Spacer());
                    statsDisplays.Add(new TextControl{ Parameters = ItemStatsDescriber.Describe(entity) });
                }

                statsDisplays.Add(new Spacer());
            }

            if (entity.Has<Experience>())
            {
                statsDisplays.Add(new ExperienceControl());
            }

            if (entity.Has<Health>())
            {
                statsDisplays.AddRange(GetCombatStats(entity));
            }

            if (entity.Has<Equipped>())
            {
                statsDisplays.Add(new Spacer());
                statsDisplays.Add(new EquippedItemsListControl());
            }


            return new List<IDataRogueControl>
            {
                new FlowContainerControl
                {
                    Position = new Rectangle(0,0,0,0),
                    Controls = statsDisplays
                }
            };
        }

        public static List<IDataRogueControl> GetCombatStats(IEntity entity)
        {
            var ret = new List<IDataRogueControl>();

            var healthStats = new List<IDataRogueControl>
            {
                new ComponentCounter{ Parameters = "Health,HP", BackColor = Color.DarkRed },
                new Spacer()
            };

            var auraStats = new List<IDataRogueControl>
            {
                new ComponentCounter{ Parameters = "AuraFighter,Aura", BackColor = Color.Yellow}
            };

            if (entity.IsPlayer)
            {
                auraStats.Add(new StatControl{ Parameters = "Tension" });
            }
            auraStats.Add(new Spacer());

            var aegisStats = new List<IDataRogueControl>()
            {
                new StatControl{ Parameters = "CurrentAegisLevel"},
                new StatControl{ Parameters = "Aegis"},
                new Spacer(),
            };

            var tiltStats =  new List<IDataRogueControl>
            {
                new ComponentCounter{ Parameters = "TiltFighter,Tilt", BackColor = Color.Purple },
                new Spacer()
            };

            var combatStats = new List<IDataRogueControl>
            {
                new StatControl{ Parameters = "Muscle"},
                new StatControl{ Parameters = "Agility"},
                new StatControl{ Parameters = "Intellect"},
                new StatControl{ Parameters = "Willpower"},
                new Spacer(),
                new StatControl{ Parameters = "AC"},
                new StatControl{ Parameters = "EV"},
                new StatControl{ Parameters = "SH"},
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