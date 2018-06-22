namespace data_rogue_core.Entities
{
    public class AuraCounter : CounterBase
    {
        public AuraCounter(int counterValue) : base(counterValue, 100)
        {
        }

        public int CurrentAura => CounterValue;
        public int MaxAura => CounterMax;
    }
}