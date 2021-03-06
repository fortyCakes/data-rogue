﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_one.EventSystem.Utils
{
    public static class MorgueHelper
    {
        public static List<IDataRogueControl> GetStatusConfigurations(IEntity player)
        {
            var statsDisplays = new List<IDataRogueControl>
            {
                new LargeTextControl { Parameters = "You have died." },
                new Spacer(),
                new AppearanceName { Entity = player },
                new Spacer()
            };


            statsDisplays.Add(new TextControl { Parameters = player.Get<Description>().Detail });
            statsDisplays.Add(new ExperienceControl { Entity = player });
            statsDisplays.AddRange(GetCombatStats(player));


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

            var healthStats = new List<IDataRogueControl> {
                new ComponentCounter{ Parameters = "Health,HP", BackColor = Color.DarkRed, Entity = entity },
                new Spacer()
            };

            var auraStats = new List<IDataRogueControl>
            {
                new ComponentCounter { Parameters = "AuraFighter,Aura", BackColor = Color.Yellow, Entity = entity },
                new Spacer()
            };

            var aegisStats = new List<IDataRogueControl>()
            {
                new StatControl { Parameters = "CurrentAegisLevel", Entity = entity },
                new StatControl { Parameters = "Aegis", Entity = entity },
                new Spacer(),
            };

            var tiltStats =  new List<IDataRogueControl>
            {
                new ComponentCounter { Parameters = "TiltFighter,Tilt", BackColor = Color.Purple, Entity = entity },
                new Spacer()
            };

            var combatStats = new List<IDataRogueControl>
            {
                new StatControl { Parameters = "Muscle", Entity = entity},
                new StatControl { Parameters = "Agility", Entity = entity},
                new StatControl { Parameters = "Intellect", Entity = entity},
                new StatControl { Parameters = "Willpower", Entity = entity},
                new Spacer(),
                new StatControl { Parameters = "AC", Entity = entity},
                new StatControl { Parameters = "EV", Entity = entity},
                new StatControl { Parameters = "SH", Entity = entity},
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
            text.AppendLine($" Score: {systemContainer.EventSystem.GetStat(player, "Score")}");
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