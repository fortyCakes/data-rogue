using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EntityEngineSystem
{
    public class EntityReferenceList : IList<uint>, ICustomFieldSerialization
    {
        private List<uint> _internalList = new List<uint>();

        public uint this[int index] { get => ((IList<uint>)_internalList)[index]; set => ((IList<uint>)_internalList)[index] = value; }

        public int Count => ((IList<uint>)_internalList).Count;

        public bool IsReadOnly => ((IList<uint>)_internalList).IsReadOnly;

        public void Add(uint item)
        {
            ((IList<uint>)_internalList).Add(item);
        }

        public void Add(IEntity entity)
        {
            Add(entity.EntityId);
        }

        public void Clear()
        {
            ((IList<uint>)_internalList).Clear();
        }

        public bool Contains(uint item)
        {
            return ((IList<uint>)_internalList).Contains(item);
        }

        public bool Contains(IEntity entity)
        {
            return Contains(entity.EntityId);
        }

        public void CopyTo(uint[] array, int arrayIndex)
        {
            ((IList<uint>)_internalList).CopyTo(array, arrayIndex);
        }

        public void Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _internalList = new List<uint>();
                return;
            }

            var splits = value.Split(',');

            _internalList = new List<uint>(splits.Select(s => uint.Parse(s)));
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return ((IList<uint>)_internalList).GetEnumerator();
        }

        public int IndexOf(uint item)
        {
            return ((IList<uint>)_internalList).IndexOf(item);
        }

        public int IndexOf(IEntity entity)
        {
            return IndexOf(entity.EntityId);
        }

        public void Insert(int index, uint item)
        {
            ((IList<uint>)_internalList).Insert(index, item);
        }

        public void Insert(int index, IEntity entity)
        {
            Insert(index, entity.EntityId);
        }

        public bool Remove(uint item)
        {
            return ((IList<uint>)_internalList).Remove(item);
        }

        public bool Remove(IEntity entity)
        {
            return Remove(entity.EntityId);
        }

        public void RemoveAt(int index)
        {
            ((IList<uint>)_internalList).RemoveAt(index);
        }

        public string Serialize()
        {
            return string.Join(",", this.Select(id => id.ToString()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<uint>)_internalList).GetEnumerator();
        }
    }
}