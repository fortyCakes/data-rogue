using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{

    public class RangedAttackAction : ApplyActionRule
    {
        public virtual List<string> RangedAttackClasses => new List<string> { "Launcher", "Thrown", "Blast", "Bolt" };

        public RangedAttackAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.RangedAttack;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            if (!AttackerCanMakeRangedAttack(sender))
            {
                return false;
            }

            _systemContainer.TargetingSystem.GetTarget(sender, new TargetingData { Range = 10 }, (mapCoordinate) => ResolveRangedAttack(sender, mapCoordinate));
            
            return false;
        }

        private bool AttackerCanMakeRangedAttack(IEntity attacker)
        {
            var weapon = GetFirstRangedWeapon(attacker);

            if (weapon == null)
            {
                if (_systemContainer.PlayerSystem.IsPlayer(attacker))
                {
                    _systemContainer.MessageSystem.Write("You can't make a ranged attack without a ranged weapon.");
                }

                return false;
            }

            return WeaponHasEnoughAmmunition(weapon, attacker);
        }

        private bool WeaponHasEnoughAmmunition(IEntity weapon, IEntity sender)
        {
            if (!weapon.Has<RequiresAmmunition>())
            {
                return true;
            }

            if (!sender.Has<Inventory>())
            {
                return false;
            }

            var ammunitionType = weapon.Get<RequiresAmmunition>().AmmunitionType;

            var inventory = _systemContainer.ItemSystem.GetInventory(sender);

            var hasAmmo = inventory.Any(i => i.Has<Ammunition>() && i.Get<Ammunition>().AmmunitionType == ammunitionType);

            if (!hasAmmo && _systemContainer.PlayerSystem.IsPlayer(sender))
            {
                _systemContainer.MessageSystem.Write($"You can't attack with {weapon.DescriptionName} without [{ammunitionType}].");
            }

            return hasAmmo;
        }

        private IEntity GetFirstRangedWeapon(IEntity attacker)
        {
            if (!attacker.Has<Equipped>())
            {
                return null;
            }

            var equipped = attacker.Get<Equipped>();
            var equipment = equipped.EquippedItems.Select(e => _systemContainer.EntityEngine.Get(e.EquipmentId));

            var weapons = equipment.Where(e => e.Has<Weapon>() && RangedAttackClasses.Contains(e.Get<Weapon>().Class));            

            return weapons.OrderBy(e => equipped.EquippedItems.Single(eq => eq.EquipmentId == e.EntityId).Slot.ToString()).FirstOrDefault();
        }

        private void ResolveRangedAttack(IEntity attacker, MapCoordinate mapCoordinate)
        {
            var defender = _systemContainer.PositionSystem
                .EntitiesAt(mapCoordinate)
                .FirstOrDefault(e => e.Has<Health>());

            if (defender != null)
            {
                var weapon = GetFirstRangedWeapon(attacker);
                var weaponClass = GetRangedWeaponClass(weapon);

                _systemContainer.FighterSystem.Attack(attacker, defender, attackClass: weaponClass, weapon: weapon, spendTime: true);

                ExpendAmmoIfRequired(attacker, weapon);
            }
            else
            {
                _systemContainer.MessageSystem.Write("No valid target there.");
            }
        }

        private void ExpendAmmoIfRequired(IEntity attacker, IEntity weapon)
        {
            if (weapon.Has<RequiresAmmunition>())
            {
                var ammunitionType = weapon.Get<RequiresAmmunition>().AmmunitionType;
                var inventory = _systemContainer.ItemSystem.GetInventory(attacker);

                var ammo = _systemContainer.ItemSystem.GetInventory(attacker).Single(i => i.Has<Ammunition>() && i.Get<Ammunition>().AmmunitionType == ammunitionType);

                _systemContainer.ItemSystem.RemoveItemFromInventory(ammo);
            }
        }

        private string GetRangedWeaponClass(IEntity weapon)
        {
            return weapon.Get<Weapon>().Class;
        }
    }
}
