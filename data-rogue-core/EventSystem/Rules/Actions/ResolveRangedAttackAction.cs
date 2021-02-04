using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{

    public class ResolveRangedAttackAction : ApplyActionRule
    {
        public ResolveRangedAttackAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.ResolveRangedAttack;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var splits = eventData.Parameters.Split(',');
            var attacker = _systemContainer.EntityEngine.Get(uint.Parse(splits[0]));
            var defender = _systemContainer.EntityEngine.Get(uint.Parse(splits[1]));
            var weapon = _systemContainer.EntityEngine.Get(uint.Parse(splits[2]));
            var weaponClass = GetRangedWeaponClass(weapon);

            _systemContainer.FighterSystem.Attack(attacker, defender, attackClass: weaponClass, weapon: weapon, spendTime: true);

            ExpendAmmoIfRequired(attacker, weapon);

            eventData.IsAction = true;

            return true;
        }

        private void ExpendAmmoIfRequired(IEntity attacker, IEntity weapon)
        {
            if (weapon.Has<RequiresAmmunition>())
            {
                var ammunitionType = weapon.Get<RequiresAmmunition>().AmmunitionType;
                var inventory = _systemContainer.ItemSystem.GetInventory(attacker);

                var ammo = _systemContainer.ItemSystem.GetInventory(attacker).Single(i => i.Has<Ammunition>() && i.Get<Ammunition>().AmmunitionType == ammunitionType);

                _systemContainer.ItemSystem.DestroyItem(ammo, false);
            }
        }

        private string GetRangedWeaponClass(IEntity weapon)
        {
            return weapon.Get<Weapon>().Class;
        }
    }
}
