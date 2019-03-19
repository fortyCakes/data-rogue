using System;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class ApplyHitOrMissedAttackRule: IEventRule
    {
        private ISystemContainer _systemContainer;

        public ApplyHitOrMissedAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.SuccessfulDefenceType == null)
            {
                OnHit(data);
                return true;
            }
            else
            {
                OnMiss(data);
                return false;
            }
        }

        private void OnHit(AttackEventData data)
        {
            var damageData = new DamageEventData { Damage = data.Damage.Value, DamagedBy = data.Attacker};

            var damaged = _systemContainer.EventSystem.Try(EventType.Damage, data.Defender, damageData);

            if (damaged)
            {
                DescribeSuccessfulAttack(data);
            }
            else
            {
                if (damageData.Absorbed)
                {
                    data.SuccessfulDefenceType = "Absorbed";
                }
                else
                {
                    data.SuccessfulDefenceType = "NoDamage";
                }

                DescribeMissedAttack(data);
            }
        }

        private void DescribeSuccessfulAttack(AttackEventData data)
        {
            string message = GetBaseAttackMessage(data);

            message += $" and hits for {data.Damage} damage.";

            _systemContainer.MessageSystem.Write(message);
        }

        private void OnMiss(AttackEventData data)
        {
            DescribeMissedAttack(data);
        }

        private void DescribeMissedAttack(AttackEventData data)
        {
            string message = GetBaseAttackMessage(data);

            switch (data.SuccessfulDefenceType)
            {
                case "Block":
                    message += " but they block the attack.";
                    break;
                case "Dodge":
                    message += " but they dodge.";
                    break;
                case "Tank":
                    message += " but it's stopped by their armour.";
                    break;
                case "NoDamage":
                    message += " and hits, but it doesn't deal any damage.";
                    break;
                case "Absorbed":
                    message += " and hits, but is absorbed.";
                    break;
                default:
                    throw new ApplicationException("Unknown defence type in DescribeMissedAttack");
            }

            _systemContainer.MessageSystem.Write(message);
        }

        private static string GetBaseAttackMessage(AttackEventData data)
        {
            var attackerName = data.Attacker.DescriptionName;
            var defenderName = data.Defender.DescriptionName;
            
            return $"{attackerName} attacks {defenderName}";
        }
    }
}
