using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_one.Utils;
using data_rogue_one.World.GenerationStrategies.ItemGeneration;

namespace data_rogue_one.EventSystem.Utils
{
    public class ItemStatsDescriber
    {
        public static string Describe(IEntity item)
        {
            StringBuilder sb = new StringBuilder();

            var itemClass = ItemClassHelper.Classify(item);
            var stats = item.Components.OfType<Stat>();

            switch(itemClass)
            {
                case ItemClass.Other:
                    break;
                case ItemClass.Shield:
                case ItemClass.Amulet:
                case ItemClass.Ring:
                case ItemClass.Boots:
                case ItemClass.Gloves:
                    AppendStandardLines(itemClass.ToString(), item, sb, stats);
                    break;
                case ItemClass.HeavyWeapon:
                    AppendStandardLines("Heavy Weapon", item, sb, stats);
                    break;
                case ItemClass.LightWeapon:
                    AppendStandardLines("Light Weapon", item, sb, stats);
                    break;
                case ItemClass.RangedWeapon:
                    AppendStandardLines("Ranged Weapon", item, sb, stats);
                    break;
                case ItemClass.MagicWeapon:
                    AppendStandardLines("Magic Weapon", item, sb, stats);
                    break;
                case ItemClass.Head:
                    AppendStandardLines("Head Armour", item, sb, stats);
                    break;
                case ItemClass.Body:
                    AppendStandardLines("Body Armour", item, sb, stats);
                    break;
                case ItemClass.Legs:
                    AppendStandardLines("Leg Armour", item, sb, stats);
                    break;
                case ItemClass.Ammunition:
                    var ammunition = item.Get<Ammunition>();
                    var stackable = item.Get<Stackable>();
                    sb.AppendLine("Ammunition");
                    sb.AppendLine($"Ammunition Type: {ammunition.AmmunitionType}");
                    sb.AppendLine($"Amount: {stackable.StackSize}");
                    break;
                case ItemClass.Potion:
                    sb.AppendLine("Potion");
                    break;
                case ItemClass.SkillBook:
                    var skill = item.Get<KnownSkill>();
                    sb.AppendLine("Skill Book");
                    sb.AppendLine("Teaches " + skill.Skill);
                    break;
                case ItemClass.Wealth:
                    var wealth = item.Get<Wealth>();
                    sb.AppendLine("Wealth");
                    sb.AppendLine($"Value: {wealth.Amount} {wealth.Currency}");
                    break;
                default:
                    throw new ApplicationException($"Can't describe item: unknown item class {itemClass}");
            }

            return sb.ToString();
        }

        private static void AppendStandardLines(string itemClassName, IEntity item, StringBuilder sb, IEnumerable<Stat> stats)
        {
            sb.AppendLine(itemClassName);
            AddEquipmentSlots(sb, item);
            AddBaseStats(sb, stats);
        }

        private static void AddEquipmentSlots(StringBuilder sb, IEntity item)
        {
            var slots = item.Components.OfType<Equipment>();
            var requiredSlots = slots.Select(s => s.EquipmentSlot).ToList();
            var additionalSlots = slots.Where(s => s.AdditionalEquipmentSlot != null).Select(s => s.AdditionalEquipmentSlot.Value);
            requiredSlots.AddRange(additionalSlots);

            var grouped = requiredSlots.GroupBy(s => s.ToString());

            var text = string.Join(",", grouped.Select(s => DescribeGroupedSlotAmount(s)));

            sb.AppendLine($"Equipment Slot: {text}");
        }

        private static string DescribeGroupedSlotAmount(IGrouping<string, EquipmentSlot> s)
        {
            return (s.Count() > 1 ? (s.Count() + "x ") : "") + s.Key;
        }

        private static void AddBaseStats(StringBuilder sb, IEnumerable<Stat> stats)
        {
            AddStatLine(sb, stats, "Accuracy");
            AddStatLine(sb, stats, "Damage");
            AddStatLine(sb, stats, "Speed");
            AddStatLine(sb, stats, "Intellect");
            AddStatLine(sb, stats, "Willpower");
            AddStatLine(sb, stats, "Aegis");
            AddStatLine(sb, stats, "AC", "Armour Class");
            AddStatLine(sb, stats, "SH", "Shield Block");
            AddStatLine(sb, stats, "EV", "Evasion");
        }

        private static void AddStatLine(StringBuilder sb, IEnumerable<Stat> stats, string statName, string statNameOverride = null)
        {
            var stat = stats.SingleOrDefault(s => s.Name == statName);

            if (stat != null)
            {
                sb.AppendLine($"{ statNameOverride ?? statName }: {stat.Value}");
            }
        }
    }
}