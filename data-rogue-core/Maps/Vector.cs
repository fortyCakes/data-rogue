using System;

namespace data_rogue_core.Maps
{
    public class Vector
    {
        public int X;
        public int Y;

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Vector other = obj as Vector;

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Equals(other);
        }

        protected bool Equals(Vector other)
        {
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static Vector operator +(Vector thisVector, Vector thatVector)
        {
            return new Vector(thisVector.X + thatVector.X, thisVector.Y + thatVector.Y);
        }
        public static Vector operator -(Vector thisVector, Vector thatVector)
        {
            return new Vector(thisVector.X - thatVector.X, thisVector.Y - thatVector.Y);
        }

        public static bool operator == (Vector thisVector, Vector thatVector)
        {
            if (ReferenceEquals(thisVector, null)) return false;
            if (ReferenceEquals(thatVector, null)) return false;

            return thisVector.Equals(thatVector);
        }

        public static Vector Parse(string parameters)
        {
            var splits = parameters.Split(',');

            var x = int.Parse(splits[0]);
            var y = int.Parse(splits[1]);

            return new Vector(x, y);
        }

        public static bool operator !=(Vector thisVector, Vector thatVector)
        {
            if (ReferenceEquals(thisVector, null)) return false;
            if (ReferenceEquals(thatVector, null)) return false;
            return !(thisVector == thatVector);
        }

        public static Vector operator /(Vector thisVector, int divideBy)
        {
            if (ReferenceEquals(thisVector, null)) return null;
            return new Vector(thisVector.X / divideBy, thisVector.Y / divideBy);
        }

        public Vector Transpose()
        {
            return new Vector(Y, X);
        }

        public static Vector[] GetAdjacentCellVectors()
        {
            return new[]
            {
                new Vector(-1, 0),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, 0)
            };
        }

        public static Vector[] GetAdjacentAndDiagonalCellVectors()
        {
            return new[]
            {
                new Vector(-1, -1), new Vector(-1, 0), new Vector(-1, 1),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, -1), new Vector(1, 0), new Vector(1, 1)
            };
        }
    }
}