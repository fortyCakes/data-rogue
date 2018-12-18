using data_rogue_core.EntitySystem;
using System.Text.RegularExpressions;

namespace data_rogue_core.Maps
{
    public class MapCoordinate : ICustomFieldSerialization
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

        public MapCoordinate()
        {
        }

        public MapCoordinate(MapKey mapKey, Vector vector)
        {
            Key = mapKey;
            X = vector.X;
            Y = vector.Y;
        }

        public override bool Equals(object obj)
        {
            MapCoordinate other = obj as MapCoordinate;

            if (ReferenceEquals(other, null)) return false;

            return Key?.Key == other.Key?.Key && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ X ^ (Y << 16);
        }

        public override string ToString()
        {
            return $"Key: {Key}, X: {X}, Y: {Y}";
        }

        public string Serialize()
        {
            return this.ToString();
        }

        public void Deserialize(string value)
        {
            var match = Regex.Match(value, "^Key: (.*), X: (-?[0-9]*), Y: (-?[0-9]*)$");
            Key = new MapKey(match.Groups[1].Value);
            X = int.Parse(match.Groups[2].Value);
            Y = int.Parse(match.Groups[3].Value);
        }

        public static bool operator== (MapCoordinate a, MapCoordinate b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            if (ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(MapCoordinate a, MapCoordinate b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
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