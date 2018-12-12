namespace data_rogue_core.Maps
{
    public class Vector
    {
        public int X;
        public int Y;

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

            return other.X == this.X && other.Y == this.Y;
        }
    }
}