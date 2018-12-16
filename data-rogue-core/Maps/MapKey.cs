using System;

namespace data_rogue_core.Maps
{
    [Serializable]
    public class MapKey 
    {
        public string Key { get; set; }

        public MapKey(string key)
        {
            Key = key;
        }

        public override bool Equals(object obj)
        {
            MapKey other = obj as MapKey;

            if (other == null) return false;

            return Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Key;
        }

        public static bool operator ==(MapKey a, MapKey b)
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

        public static bool operator !=(MapKey a, MapKey b)
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
    }
}