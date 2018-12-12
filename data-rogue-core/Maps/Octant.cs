using System.Collections.Generic;

namespace data_rogue_core.Maps
{
    public class Octant
    {
        public static readonly List<Octant> AllOctants = new List<Octant>
        {
            new Octant(new Matrix(+1, 0, 0, +1)),
            new Octant(new Matrix(-1, 0, 0, +1)),
            new Octant(new Matrix(+1, 0, 0, -1)),
            new Octant(new Matrix(-1, 0, 0, -1)),
            new Octant(new Matrix(0, +1, +1, 0)),
            new Octant(new Matrix(0, -1, +1, 0)),
            new Octant(new Matrix(0, +1, -1, 0)),
            new Octant(new Matrix(0, -1, -1, 0))
        };

        public Matrix Matrix { get; }

        private Octant(Matrix coordinateTransform)
        {
            Matrix = coordinateTransform;
        }
    }
}