using data_rogue_core.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.EntityEngineSystem
{
    public class EquipmentMappingList : IList<EquipmentMappingListItem>, ICustomFieldSerialization
    {
        private List<EquipmentMappingListItem> items;

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

    public class EquipmentMappingListItem : ICustomFieldSerialization
    {
        public EquipmentSlotDetails Slot;
        public uint EquipmentId;

        public void Deserialize(string value)
        {
            var splits = value.Split(':');
            Slot = new EquipmentSlotDetails();
            Slot.Deserialize(splits[0]);
            EquipmentId = uint.Parse(splits[1]);
        }

        public override bool Equals(object obj)
        {
            var asItem = obj as EquipmentMappingListItem;

            if (asItem == null) return false;

            return Slot == asItem.Slot && EquipmentId == asItem.EquipmentId;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 7 + Slot.GetHashCode();
            hash = hash * 7 + EquipmentId.GetHashCode();

            return hash;
        }

        public string Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Slot.Serialize()}:{EquipmentId}";
        }

        public static bool operator ==(EquipmentMappingListItem a, EquipmentMappingListItem b)
        {
            var aItem = a as EquipmentMappingListItem;
            var bItem = b as EquipmentMappingListItem;

            if (aItem == null || bItem == null) return false;

            return aItem.Equals(bItem);
        }

        public static bool operator !=(EquipmentMappingListItem a, EquipmentMappingListItem b)
        {
            var aItem = a as EquipmentMappingListItem;
            var bItem = b as EquipmentMappingListItem;

            if (aItem == null || bItem == null) return false;

            return !aItem.Equals(bItem);
        }
    }

    public class EquipmentSlotDetails : ICustomFieldSerialization
    {
        public BodyPartType BodyPartType;
        public BodyPartLocation BodyPartLocation;
        public int Index = 1;

        public void Deserialize(string value)
        {
            var splits = value.Split('|');

            BodyPartType = (BodyPartType)Enum.Parse(typeof(BodyPartType), splits[0]);
            BodyPartLocation = (BodyPartLocation)Enum.Parse(typeof(BodyPartLocation), splits[1]);
            Index = int.Parse(splits[2]);
        }

        public string Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{BodyPartType}|{BodyPartLocation}|{Index}";
        }
    }
}
