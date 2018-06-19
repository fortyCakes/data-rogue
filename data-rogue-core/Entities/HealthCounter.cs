using System;

namespace data_rogue_core.Entities
{
    public class HealthCounter : CounterBase
    {
        public HealthCounter(int counterValue, int? counterMax = null) : base(counterValue, counterMax)
        {
        }

        public int CurrentHealth => CounterValue;
        public int MaxHealth => CounterMax;
    }
}