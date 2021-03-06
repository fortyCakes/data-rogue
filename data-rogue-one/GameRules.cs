﻿using data_rogue_core.EventSystem.Rules;
using data_rogue_one.EventSystem.Rules;
using System;
using System.Collections.Generic;
using data_rogue_one.Rules;

namespace data_rogue_one
{

    public static class GameRules
    {
        public static List<Type> Rules
        {
            get
            {
                var list = new List<Type> {
                    typeof(PhysicalCollisionRule),
                    typeof(BumpAttackRule),
                    typeof(BumpInteractRule),
                    typeof(BranchGeneratorRule),
                    typeof(SetAttackClassOnAttackRule),
                    typeof(SpendTimeOnActionRule),
                    typeof(SetDamageOnAttackRule),
                    typeof(SetWeaponOnAttackRule),
                    typeof(DealDamageRule),
                    typeof(CantDealNoDamageRule),
                    typeof(PeopleDieWhenTheyAreKilledRule),
                    typeof(PeopleDropTheirStuffWhenTheyAreKilledRule),
                    typeof(PeopleUnequipTheirStuffWhenTheyAreKilledRule),
                    typeof(DestroyCommonItemsOnDeathRule),
                    typeof(SpendTimeRule),
                    typeof(ReturnToMenuOnDeathRule),
                    typeof(PlayerVictoryRule),
                    typeof(CompleteMoveRule),
                    typeof(GetBaseStatRule),
                    typeof(EnemiesInViewAddTensionRule),
                    typeof(CheckEnoughAuraToActivateSkillRule),
                    typeof(ApplyStatBoostEnchantmentRule),
                    typeof(SpendAuraOnCompleteSkillRule),
                    typeof(SpendTimeOnCompleteSkillRule),
                    typeof(DoXpGainRule),
                    typeof(LevelUpOnXPGainRule),
                    typeof(IncreaseStatsOnLevelUpRule),
                    typeof(GainSingleXPOnKillRule),
                    typeof(ApplyEquipmentStatsRule),
                    typeof(SetSpeedOnAttackRule),
                    typeof(SetAccuracyOnAttackRule),
                    typeof(UnrolledAccuracyRule),
                    typeof(TryApplyBlockOnAttackRule),
                    typeof(TryApplyDodgeOnAttackRule),
                    typeof(TryApplyTankOnAttackRule),
                    typeof(ApplyTiltBlockDefenceRule),
                    typeof(ApplyTiltDodgeDefenceRule),
                    typeof(ApplyTiltTankDefenceRule),
                    typeof(ApplyHitOrMissedAttackRule),
                    typeof(AddAgilityToEvasionRule),
                    typeof(DefaultSpeedRule),
                    typeof(RandomiseDamageRule),
                    typeof(ApplyAegisRule),
                    typeof(AddStatToAccuracyRule),
                    typeof(OnAttackProcEnchantmentRule),
                    typeof(ApplyResistanceRule),
                    typeof(ApplyModifiedAccuracyRule),
                    typeof(ApplyModifiedDamageRule),
                    typeof(ShowToastWhenEnteringBranchRule),
                    typeof(PlayerStepMakesSoundRule),
                    typeof(HitMakesSoundRule),
                    typeof(MissMakesSoundRule),
                    typeof(AttackAnimationRule),
                    typeof(DamageNumbersPopOutRule),
                    typeof(DamageSplatParticlesRule),
                    typeof(DisplayAdvancedMorgueScreenOnDeathRule),
                    typeof(SaveMorgueFileOnDeathRule),
                    typeof(GoldAddsScoreRule),
                    typeof(XPAddsScoreRule),
                    typeof(ExplorationAddsScoreRule),
                    typeof(UpdateHighScoreFileOnDeathRule),
                    typeof(PreventDamageIfGodModeRule),
                    typeof(CantAffordItemRule),
                    typeof(CompletePurchaseRule),
                    typeof(CantMoveIntoSameFactionRule),
                    typeof(MoveThroughPortalRule),
                    typeof(SpawnedMonsterCanHaveEnchantedGearRule)
                };

                list.AddRange(ApplyActionRule.AllActionRules);

                // Custom actions
                var customActions = new List<Type>
                {
                    typeof(RestAction),
                    typeof(ExamineStatusAction),
                    typeof(ExamineStatusFromMenuAction),
                    typeof(PlayerStatusAction),
                    typeof(MorgueAction)
                };

                list.AddRange(customActions);

                return list;
            }
        }
    }
}
