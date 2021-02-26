using System.Collections;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Data
{
    [AlwaysCreateNewInstance]
    public class EquipmentMappingList : IList<EquipmentMappingListItem>, ICustomFieldSerialization
    {
        private List<EquipmentMappingListItem> items = new List<EquipmentMappingListItem>();

        public EquipmentMappingListItem this[int index] { get => ((IList<EquipmentMappingListItem>)items)[index]; set => ((IList<EquipmentMappingListItem>)items)[index] = value; }

        public int Count => ((IList<EquipmentMappingListItem>)items).Count;

        public bool IsReadOnly => ((IList<EquipmentMappingListItem>)items).IsReadOnly;

        public void Add(EquipmentMappingListItem item)
        {
            ((IList<EquipmentMappingListItem>)items).Add(item);
        }

        public void Clear()
        {
            ((IList<EquipmentMappingListItem>)items).Clear();
        }

        public bool Contains(EquipmentMappingListItem item)
        {
            return ((IList<EquipmentMappingListItem>)items).Contains(item);
        }

        public void CopyTo(EquipmentMappingListItem[] array, int arrayIndex)
        {
            ((IList<EquipmentMappingListItem>)items).CopyTo(array, arrayIndex);
        }

        public void Deserialize(string value)
        {
            items = new List<EquipmentMappingListItem>();

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var splits = value.Split(',');

            foreach (string s in splits)
            {
                var item = new EquipmentMappingListItem();
                item.Deserialize(s);
                items.Add(item);
            }
        }

        public IEnumerator<EquipmentMappingListItem> GetEnumerator()
        {
            return ((IList<EquipmentMappingListItem>)items).GetEnumerator();
        }

        public int IndexOf(EquipmentMappingListItem item)
        {
            return ((IList<EquipmentMappingListItem>)items).IndexOf(item);
        }

        public void Insert(int index, EquipmentMappingListItem item)
        {
            ((IList<EquipmentMappingListItem>)items).Insert(index, item);
        }

        public bool Remove(EquipmentMappingListItem item)
        {
            return ((IList<EquipmentMappingListItem>)items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<EquipmentMappingListItem>)items).RemoveAt(index);
        }

        public string Serialize()
        {
            return string.Join(",", items.Select(e => e.Serialize()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<EquipmentMappingListItem>)items).GetEnumerator();
        }
    }
}