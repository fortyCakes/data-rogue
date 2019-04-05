using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class OnAttackProcEnchantmentRule : ApplyProcEnchantmentRule
    {
        public OnAttackProcEnchantmentRule(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override EventType EventType => EventType.Attack;
    }
}