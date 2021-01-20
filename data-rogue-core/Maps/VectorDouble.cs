namespace data_rogue_core.Maps
{
    public class VectorDouble
    {
        public double X;
        public double Y;

        public VectorDouble(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            VectorDouble other = obj as VectorDouble;

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Equals(other);
        }

        protected bool Equals(VectorDouble other)
        {
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static VectorDouble operator +(VectorDouble thisVector, VectorDouble thatVector)
        {
            return new VectorDouble(thisVector.X + thatVector.X, thisVector.Y + thatVector.Y);
        }

        public static bool operator == (VectorDouble thisVector, VectorDouble thatVector)
        {
            if (ReferenceEquals(thisVector, null)) return false;
            if (ReferenceEquals(thatVector, null)) return false;

            return thisVector.Equals(thatVector);
        }

        public static VectorDouble Parse(string parameters)
        {
            var splits = parameters.Split(',');

            var x = double.Parse(splits[0]);
            var y = double.Parse(splits[1]);

            return new VectorDouble(x, y);
        }

        public static bool operator !=(VectorDouble thisVector, VectorDouble thatVector)
        {
            if (ReferenceEquals(thisVector, null)) return false;
            if (ReferenceEquals(thatVector, null)) return false;
            return !(thisVector == thatVector);
        }

        public static VectorDouble operator /(VectorDouble thisVector, double divideBy)
        {
            if (ReferenceEquals(thisVector, null)) return null;
            return new VectorDouble(thisVector.X / divideBy, thisVector.Y / divideBy);
        }

        public VectorDouble Transpose()
        {
            return new VectorDouble(Y, X);
        }

        public static implicit operator VectorDouble(Vector vector)
        {
            return new VectorDouble(vector.X, vector.Y);
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}