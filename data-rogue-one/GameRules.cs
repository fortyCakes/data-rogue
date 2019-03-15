using data_rogue_core;
using data_rogue_core.EventSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one
{

    public static class GameRules
    {
        public static List<Type> Rules
        {
            get
            {
                return new List<Type> {
                    typeof(InputHandlerRule),
                    typeof(PhysicalCollisionRule),
                    typeof(BumpAttackRule),
                    typeof(BranchGeneratorRule),
                    typeof(SetAttackClassOnAttackRule),
                    typeof(SpendTimeOnAttackRule),
                    typeof(SetDamageOnAttackRule),
                    typeof(SetWeaponOnAttackRule),
                    typeof(DealDamageRule),
                    typeof(PeopleDieWhenTheyAreKilledRule),
                    typeof(SpendTimeRule),
                    typeof(PlayerDeathRule),
                    typeof(CompleteMoveRule),
                    typeof(GetBaseStatRule),
                    typeof(EnemiesInViewAddTensionRule),
                    typeof(CheckEnoughAuraToActivateSkillRule),
                    typeof(ApplyStatBoostEnchantmentRule),
                    typeof(SpendAuraOnCompleteSkillRule),
                    typeof(SpendTimeOnCompleteSkillRule),
                    typeof(DoXpGainRule),
                    typeof(DisplayMessageOnXPGainRule),
                    typeof(LevelUpOnXPGainRule),
                    typeof(GainSingleXPOnKillRule),
                    typeof(ApplyEquipmentStatsRule),
                    typeof(SetSpeedOnAttackRule),
                    typeof(SetAccuracyOnAttackRule),
                    typeof(RollAccuracyOnAttackRule),
                    typeof(TryApplyBlockOnAttackRule),
                    typeof(TryApplyDodgeOnAttackRule),
                    typeof(TryApplyTankOnAttackRule),
                    typeof(ApplyBlockDefenceRule),
                    typeof(ApplyDodgeDefenceRule),
                    typeof(ApplyTankDefenceRule),
                    typeof(ApplyHitOrMissedAttackRule),
                    typeof(AddAgilityToEvasionRule)
                };
            }
        }
    }
}
