using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public interface IRandom
    {
        void Seed(string seed);
        int Between(int min, int max);

        T PickOne<T>(IList<T> items);

        decimal StatCheck(decimal statLevel);
    }

    public class RNG : IRandom
    {
        public RNG(string seed)
        {
            Seed(seed);
        }

        private Random _random;

        private void Seed(int seed)
        {
            _random = new Random(seed);
        }

        public void Seed(string seed)
        {
            Seed(seed.GetHashCode());
        }

        public int Between(int min, int max)
        {
            return _random.Next(min, max+1);
        }

        public T PickOne<T>(IList<T> items)
        {
            var max = items.Count();
            var index = Between(1, max) - 1;
            return items[index];
        }

        public decimal StatCheck(decimal statLevel)
        {
            return Between((int)Math.Ceiling(statLevel / 2), 128 + (int)Math.Ceiling(statLevel * 1 / 4));
        }
    }
}