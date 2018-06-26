using System;

namespace data_rogue_core.Entities
{
    public class CounterBase
    {
        public CounterBase(int counterValue, int? counterMax = null)
        {
            if (!counterMax.HasValue)
            {
                counterMax = counterValue;
            }

            CounterValue = counterValue;
            CounterMax = counterMax.Value;
        }

        public int CounterValue { get; private set; }
        public int CounterMax { get; private set; }
        public override string ToString() => $"{CounterValue}/{CounterMax}";

        public void SetTo(int value)
        {
            this.CounterValue = value;
        }

        public void TakeDamage(int damage, bool canUnderflow = false)
        {
            if (damage <= 0) return;

            if (canUnderflow)
            {
                CounterValue -= damage;
            }
            else
            {
                CounterValue = Math.Max(CounterValue - damage, Math.Min(CounterValue, 0));
            }

        }

        public void Restore(int healing, bool canOverheal = false)
        {
            if (healing <= 0) return;

            if (canOverheal)
            {
                CounterValue += healing;
            }
            else
            {
                CounterValue = Math.Min(CounterValue + healing, Math.Max(CounterValue, CounterMax));
            }
        }
    }
}