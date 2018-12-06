using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace data_rogue_core.Extensions
{
    [Serializable]
    public class SDictionary<K, V> : ISerializable, IEnumerable
    {
        Dictionary<K, V> dict = new Dictionary<K, V>();

        public SDictionary() { }

        protected SDictionary(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (K key in dict.Keys)
            {
                info.AddValue(key.ToString(), dict[key]);
            }
        }

        public void Add(K key, V value)
        {
            dict.Add(key, value);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)dict).GetEnumerator();
        }

        public V this[K index]
        {
            set { dict[index] = value; }
            get { return dict[index]; }
        }
    }
}
