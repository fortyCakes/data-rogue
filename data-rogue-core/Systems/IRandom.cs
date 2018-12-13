using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public interface IRandom
    {
        void Seed(string seed);
        int Next(int min, int max);

        T PickOne<T>(List<T> items);
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

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }

        public T PickOne<T>(List<T> items)
        {
            var max = items.Count();
            var index = Next(1, max) - 1;
            return items[index];
        }
    }
}