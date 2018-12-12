using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;

namespace data_rogue_core.Maps
{
    public class ShadowcastingFovCalculator
    {
        public static HashSet<Vector> InFov(int range, Func<Vector, bool> getTransparent)
        {
            var lit = new HashSet<Vector> {new Vector(0, 0)};

            var setBag = new ConcurrentBag<HashSet<Vector>>();
            Parallel.ForEach(Octant.AllOctants, octant =>
            {
                var hashSet = new HashSet<Vector>();
                Scan(hashSet, 1, 0, 1, getTransparent, octant.Matrix, range);
                setBag.Add(hashSet);
            });

            foreach (var set in setBag)
            {
                lit.UnionWith(set);
            }

            return lit;
        }

        private static void Scan(HashSet<Vector> lit, int y, double startSlope, double endSlope, Func<Vector, bool> getTransparent, Matrix transformMatrix, int range)
        {
            if (y > range) return;
            if (startSlope >= endSlope) return;

            var xmin = (int) Math.Round((y - 0.5) * startSlope);
            var xmax = (int) Math.Ceiling((y + 0.5) * endSlope - 0.5);

            for (var x = xmin; x <= xmax; x++)
            {
                
                Vector vector = new Vector(x, y);

                Vector transformedVector = transformMatrix * vector;

                if (getTransparent(transformedVector) && IsInCircularRange(x, y, range))
                {
                    if (x >= y * startSlope && x <= y * endSlope)
                    {
                        lit.Add(transformedVector);
                    }
                }
                else
                {
                    if (x >= (y - 0.5) * startSlope && x - 0.5 <= y * endSlope)
                    {
                        lit.Add(transformedVector);
                    }

                    Scan(lit, y + 1, startSlope, (x - 0.5) / y, getTransparent, transformMatrix, range);

                    startSlope = (x + 0.5) / y;
                    if (startSlope >= endSlope)
                    {
                        return;
                    }
                }
            }

            Scan(lit, y + 1, startSlope, endSlope, getTransparent, transformMatrix, range);
        }

        private static bool IsInCircularRange(int x, int y, int range)
        {
            return x * x + y * y < range * range;
        }
    }
}