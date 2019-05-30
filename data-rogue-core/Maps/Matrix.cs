namespace data_rogue_core.Maps
{
    /// <summary>
    /// [ A1 A2 ]
    /// [ B1 B2 ]
    /// </summary>
    public class Matrix
    {
        public int A1, A2, B1, B2;

        public Matrix(int a1, int a2, int b1, int b2)
        {
            A1 = a1;
            A2 = a2;
            B1 = b1;
            B2 = b2;
        }

        public static Vector operator* (Matrix m, Vector v)
        {
            return new Vector(m.A1 * v.X + m.A2 * v.Y, m.B1 * v.X + m.B2 * v.Y);
        }

        public static Matrix Identity => new Matrix(1, 0, 0, 1);
        public static Matrix Rotate90 => new Matrix(0, -1, 1, 0);
        public static Matrix Rotate180 => new Matrix(-1, 0, 0, -1);
        public static Matrix Rotate270 => new Matrix(0, 1, -1, 0);
    }
}