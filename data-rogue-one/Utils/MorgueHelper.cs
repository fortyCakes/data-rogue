using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_one.EventSystem.Utils
{
    public static class MorgueHelper
    {
        public static List<StatsConfiguration> GetStatusConfigurations(IEntity player)
        {
            var statsDisplays = new List<InfoDisplay>
            {
                new InfoDisplay { ControlType = typeof(LargeTextControl), Parameters = "You have died." },
                new InfoDisplay { ControlType = typeof(Spacer) },
                new InfoDisplay { ControlType = typeof(AppearanceName) },
                new InfoDisplay { ControlType = typeof(Spacer) }
            };
           
            
            statsDisplays.Add(new InfoDisplay { ControlType = typeof(TextControl), Parameters = player.Get<Description>().Detail });
            statsDisplays.Add(new InfoDisplay { ControlType = typeof(ExperienceControl)});
            statsDisplays.AddRange(GetCombatStats(player));
            

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

        public static string GenerateMorgueText(ISystemContainer systemContainer)
        {
            var player = systemContainer.PlayerSystem.Player;
            var dead = player.Removed;

            StringBuilder text = new StringBuilder();

            text.AppendLine($"DataRogueOne version {Assembly.GetExecutingAssembly().GetName().Version} character file");
            text.AppendLine($"seed: {systemContainer.Seed}");
            text.AppendLine();

            text.AppendLine($"{player.Name} the Untitled, level {player.Get<Experience>().Level}");
            var startDetails = player.Get<StartDetails>();
            text.AppendLine($" Started as a {startDetails.Class} at {startDetails.StartTime}");
            text.AppendLine(" " + (dead ? "... and died" : "...and are still going!"));
            text.AppendLine();

            text.AppendLine("Stats:");
            PrintStat(systemContainer, player, text, "Muscle");
            PrintStat(systemContainer, player, text, "Agility");
            PrintStat(systemContainer, player, text, "Willpower");
            PrintStat(systemContainer, player, text, "Intellect");

            text.AppendLine();
            text.AppendLine("Skills:");

            var skills = player.Components.OfType<KnownSkill>();
            if (skills.Count() == 0)
            {
                text.AppendLine(" (no skills known)");
            }
            else
            {
                foreach (var knownskill in skills)
                {
                    var skill = systemContainer.SkillSystem.GetSkillFromKnown(knownskill);
                    var skillName = skill.DescriptionName;
                    text.AppendLine($" {skillName}");
                }
            }

            text.AppendLine();
            text.AppendLine("Equipment:");

            var equipment = systemContainer.EquipmentSystem.GetEquippedItems(player);
            if (equipment.Count() == 0)
            {
                text.AppendLine(" (no equipment)");
            }
            else
            {
                foreach (var itemEntity in equipment)
                {
                    text.AppendLine($" {itemEntity.DescriptionName}");
                }
            }



            return text.ToString();
        }

        private static void PrintStat(ISystemContainer systemContainer, IEntity player, StringBuilder text, string statName)
        {
            text.AppendLine($"{statName}: {systemContainer.StatSystem.GetEntityStat(player, statName)}");
        }
    }
}