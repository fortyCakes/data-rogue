namespace data_rogue_core.Maps
{
    public class MapCoordinate
    {
        public MapKey Key;
        public int X;
        public int Y;

        public MapCoordinate(MapKey mapKey, int x, int y)
        {
            Key = mapKey;
            X = x;
            Y = y;
        }

        public MapCoordinate(string key, int x, int y)
        {
            Key = new MapKey(key);
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            MapCoordinate other = obj as MapCoordinate;

            if (ReferenceEquals(other, null)) return false;

            return Key == other.Key && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ X ^ (Y << 16);
        }

        public override string ToString()
        {
            return $"Key: {Key}, X: {X}, Y: {Y}";
        }

        public static bool operator== (MapCoordinate a, MapCoordinate b)
        {
            if (ReferenceEquals(a, null))
            {
                return false;
            }

            if (ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(MapCoordinate a, MapCoordinate b)
        {
            if (ReferenceEquals(a, null))
            {
                return false;
            }

            if (ReferenceEquals(b, null))
            {
                return false;
            }

            return !a.Equals(b);
        }

        public static MapCoordinate operator +(MapCoordinate position, Vector vector)
        {
            var newCoordinate =  new MapCoordinate(
                    mapKey: position.Key,
                    x: position.X + vector.X,
                    y: position.Y + vector.Y
                );

            return newCoordinate;
        }
    }
}